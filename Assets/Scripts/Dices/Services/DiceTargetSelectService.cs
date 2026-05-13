using System.Collections.Generic;
using Battle;
using CustomTypes.Enums;
using CustomTypes.Enums.Battle;
using CustomTypes.Enums.Infrastructure;
using CustomTypes.Enums.Team;
using Dices.Events;
using Infrastructure;
using Leopotam.EcsLite;
using Units;
using Utils.Extensions;
using Zenject;

namespace Dices
{
    public class DiceTargetSelectService
    {
        public TeamType SelectionStartTeam => TeamType.Enemy;
        public TeamType SelectionEndTeam => TeamType.Player;
        public bool IsTargetSelecting { get; private set; }

        private readonly EcsService _ecsService;
        private readonly BattleDiceService _battleDiceService;
        private readonly DiceAimingService _aimingService;
        private readonly BattleSelectionService _selectionService;
        private readonly DiceViewService _diceViewService;
        private readonly UnitService _unitService;
        private readonly TeamService _teamService;

        private readonly EcsFilter _targetedFilter;
        private readonly EcsFilter _targetedDiceFilter;
        private readonly EcsFilter _untargetedAliveDiceFilter;
        private readonly EcsPool<TargetSelectedComponent> _targetSelectedPool;
        private readonly EcsPool<TargetedComponent> _targetedPool;
        private readonly EcsPool<DiceAimingEventComponent> _diceAimingEventPool;
        private readonly EcsPool<DiceUnaimingEventComponent> _diceUnaimingEventPool;
        private readonly EcsPool<DiceTargetedEventComponent> _diceTargetedEventPool;

        [Inject]
        public DiceTargetSelectService(EcsService ecsService, BattleDiceService battleDiceService, BattleSelectionService selectionService,
            TeamService teamService, DiceViewService diceViewService, UnitService unitService, DiceAimingService aimingService)
        {
            _ecsService = ecsService;
            _battleDiceService = battleDiceService;
            _aimingService = aimingService;
            _selectionService = selectionService;
            _diceViewService = diceViewService;
            _unitService = unitService;
            _teamService = teamService;

            _targetedFilter = ecsService.World.Filter<TargetedComponent>().End();
            _targetedDiceFilter = ecsService.World.Filter<DiceComponent>().Inc<TargetSelectedComponent>().End();
            _untargetedAliveDiceFilter = ecsService.World.Filter<DiceComponent>().Exc<DeadComponent>().Exc<TargetSelectedComponent>().End();
            _targetSelectedPool = _ecsService.World.GetPool<TargetSelectedComponent>();
            _targetedPool = _ecsService.World.GetPool<TargetedComponent>();
            _diceAimingEventPool = _ecsService.World.GetPool<DiceAimingEventComponent>();
            _diceUnaimingEventPool = _ecsService.World.GetPool<DiceUnaimingEventComponent>();
            _diceTargetedEventPool = _ecsService.World.GetPool<DiceTargetedEventComponent>();
        }

        public void StartTargetSelection()
        {
            StartTeamTargetSelection(SelectionStartTeam);
            IsTargetSelecting = true;
        }

        public void FinishTargetSelection()
        {
            LogTargets();
            _teamService.SetCurrentTeam(TeamType.None);
            IsTargetSelecting = false;
        }

        public void StartTeamTargetSelection(TeamType team)
        {
            _teamService.SetCurrentTeam(team);
            _diceViewService.ActivateCurrentTeamDices();
            TargetCurrentTeamMissedDices();
            LogService.LogDebug(DebugType.Log, $"{team}'s turn");
        }

        public void TrySetTarget(EcsPackedEntity? dicePacked)
        {
            if (!_ecsService.TryUnpack(dicePacked, out var dice) || !_aimingService.TryStopAiming(dice))
                return;

            if (!IsCurrentTargetValid(dice, out var target))
            {
                _diceUnaimingEventPool.Add(dice);
                return;
            }

            SetDiceTarget(dice, target);
            AddDiceFacetToTarget(target, dice);
        }

        public bool IsCurrentTeamTargetingFinished()
        {
            foreach (var untargetedDice in _untargetedAliveDiceFilter)
                if (_teamService.IsCurrentTeamEntity(untargetedDice))
                    return false;

            return true;
        }

        public bool IsCurrentTargetValid(int dice, out int target)
            => _selectionService.TryGetSelection(out target, out var targetType) && targetType == BattleSelectionType.Unit &&
               _battleDiceService.IsDiceUsableForTarget(dice, target);

        public void ClearUnitSelections(int unit)
        {
            RemoveUnitDiceTarget(unit);
            RemoveTargetedAtUnitDices(unit);
        }

        public void ClearAllDicesTarget()
        {
            foreach (var dice in _targetedDiceFilter)
                ClearDiceTarget(dice);
        }

        public void ClearDiceTarget(int dice)
        {
            ref var targetSelectedComponent = ref _targetSelectedPool.Get(dice);
            if (_ecsService.TryUnpack(targetSelectedComponent.Target, out var target))
            {
                ref var targetedComponent = ref _targetedPool.Get(target);
                var newDices = new Stack<EcsPackedEntity?>();
                while (targetedComponent.TargetedDices.TryPop(out var dicePacked))
                    if (_ecsService.TryUnpackWithWarning(dicePacked, out var targetedDice) && dice != targetedDice)
                        newDices.Push(dicePacked);

                if (newDices.Count == 0)
                    _targetedPool.Del(target);
                else
                    while (newDices.TryPop(out var dicePacked))
                        targetedComponent.TargetedDices.Push(dicePacked);
            }

            _targetSelectedPool.Del(dice);
        }

        private void TargetCurrentTeamMissedDices()
        {
            foreach (var dice in _untargetedAliveDiceFilter)
            {
                if (!_teamService.IsCurrentTeamEntity(dice) || _battleDiceService.GetCurrentSide(dice).SideType != DiceSideType.Miss)
                    continue;

                _targetSelectedPool.Add(dice);
                _diceAimingEventPool.Add(dice);
            }
        }

        private void SetDiceTarget(int dice, int target)
        {
            _diceTargetedEventPool.Add(dice);
            ref var targetSelectedComponent = ref _targetSelectedPool.Add(dice);
            targetSelectedComponent.Target = _ecsService.World.PackEntity(target);
        }

        private void AddDiceFacetToTarget(int target, int dice)
        {
            ref var targetedComponent = ref _targetedPool.GetOrAdd(target);
            targetedComponent.TargetedDices.Push(_ecsService.World.PackEntity(dice));
        }

        private void RemoveTargetedAtUnitDices(int unit)
        {
            if (!_targetedPool.Has(unit))
                return;

            ref var targetedComponent = ref _targetedPool.Get(unit);
            while (targetedComponent.TargetedDices.TryPop(out var packedDice))
            {
                if (!_ecsService.TryUnpack(packedDice, out var dice))
                    continue;

                _targetSelectedPool.Del(dice);
                _diceViewService.RemoveDiceFacetIcon(dice);
            }

            _targetedPool.Del(unit);
        }

        private void RemoveUnitDiceTarget(int unit)
        {
            if (!_unitService.TryGetMainDice(unit, out var dice))
                return;

            if (!_targetSelectedPool.Has(dice))
                return;

            _diceViewService.RemoveDiceFacetIcon(dice);
            ClearDiceTarget(dice);
        }

        private void LogTargets()
        {
            foreach (var targeted in _targetedFilter)
            {
                ref var targetedComponent = ref _targetedPool.Get(targeted);
                foreach (var dicePacked in targetedComponent.TargetedDices)
                {
                    _ecsService.TryUnpack(dicePacked, out var dice);
                    _battleDiceService.TryGetUnit(dice, out var unit);
                    var side = _battleDiceService.GetCurrentSide(dice);

                    LogService.LogDebug(DebugType.Log, $"Unit {unit} targeted {targeted} with dice {dice}: {side.SideType},{side.Value}");
                }
            }
        }
    }
}