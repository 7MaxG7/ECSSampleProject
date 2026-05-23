using Battle;
using CustomTypes;
using Dices;
using Infrastructure;
using Leopotam.EcsLite;
using Units;
using Utils;
using Zenject;

namespace UI.Battle
{
    public class BattleDicesUpdateUISystem : IEcsPostRunSystem
    {
        private readonly BattleUIModel _battleUIModel;
        private readonly BattleDiceLockService _diceLockService;
        private readonly BattleDiceService _battleDiceService;
        private readonly TeamService _teamService;
        private readonly UnitService _unitService;
        private readonly BattleDeathService _battleDeathService;
        private readonly DiceApplyService _diceApplyService;

        private readonly EcsFilter _battleAddedEventFilter;
        private readonly EcsFilter _diceUnaimingEventFilter;
        private readonly EcsFilter _diceAimingEventFilter;
        private readonly EcsFilter _dicesRollEventFilter;
        private readonly EcsFilter _diceLockAddedEventFilter;
        private readonly EcsFilter _diceLockDeletedEventFilter;
        private readonly EcsFilter _currentTeamModifiedEventFilter;
        private readonly EcsFilter _diceDeadAddedEventFilter;
        private readonly EcsFilter _battleSelectAddedEventFilter;
        private readonly EcsFilter _battleDeselectEventFilter;
        private readonly EcsFilter _unitFilter;
        private readonly EcsFilter _diceFilter;
        private readonly EcsPool<UnitComponent> _unitPool;
        private readonly EcsPool<TeamBattleDicesRollComponent> _teamBattleDicesRollPool;
        private readonly EcsPool<BattleSelectedComponent> _battleSelectedPool;
        private readonly EcsPool<TargetSelectedComponent> _targetSelectedPool;
        private readonly EcsPool<DiceAimingViewComponent> _diceAimingViewPool;

        [Inject]
        public BattleDicesUpdateUISystem(EcsService ecsService, BattleUIModel battleUIModel, FrameComponentsService frameComponentsService,
            BattleDiceLockService diceLockService, BattleDiceService battleDiceService, TeamService teamService, UnitService unitService,
            BattleDeathService battleDeathService, DiceApplyService diceApplyService)
        {
            _battleUIModel = battleUIModel;
            _diceLockService = diceLockService;
            _battleDiceService = battleDiceService;
            _teamService = teamService;
            _unitService = unitService;
            _battleDeathService = battleDeathService;
            _diceApplyService = diceApplyService;

            _battleAddedEventFilter = frameComponentsService.GetAddedEventFilter<BattleComponent>();
            _diceAimingEventFilter = frameComponentsService.GetEventFilter<DiceAimingEventComponent>();
            _diceUnaimingEventFilter = frameComponentsService.GetEventFilter<DiceUnaimingEventComponent>();
            _dicesRollEventFilter = frameComponentsService.GetEventFilter<DicesRollEventComponent>();
            _diceLockAddedEventFilter = frameComponentsService.GetAddedEventFilter<LockedComponent>();
            _diceLockDeletedEventFilter = frameComponentsService.GetDeletedEventFilter<LockedComponent>();
            _currentTeamModifiedEventFilter = frameComponentsService.GetModifiedEventFilter<CurrentTeamComponent>();
            _diceDeadAddedEventFilter = frameComponentsService.GetAddedEventMask<DeadComponent>().Inc<DiceComponent>().End();
            _battleSelectAddedEventFilter = frameComponentsService.GetAddedEventFilter<BattleSelectedComponent>();
            _battleDeselectEventFilter = frameComponentsService.GetEventFilter<BattleDeselectEventComponent>();
            _unitFilter = ecsService.World.Filter<UnitComponent>().End();
            _diceFilter = ecsService.World.Filter<DiceComponent>().End();
            _unitPool = ecsService.World.GetPool<UnitComponent>();
            _teamBattleDicesRollPool = ecsService.World.GetPool<TeamBattleDicesRollComponent>();
            _battleSelectedPool = ecsService.World.GetPool<BattleSelectedComponent>();
            _targetSelectedPool = ecsService.World.GetPool<TargetSelectedComponent>();
            _diceAimingViewPool = ecsService.World.GetPool<DiceAimingViewComponent>();
        }

        public void PostRun(IEcsSystems systems)
        {
            UpdateDices();
            UpdateRolls();
            UpdateLock();
            UpdateDimming();
            UpdateVisible();
            UpdateHighlight();
        }

        private void UpdateHighlight()
        {
            foreach (var selected in _battleSelectAddedEventFilter)
                ToggleSelection(selected, true);
            foreach (var deselected in _battleDeselectEventFilter)
                ToggleSelection(deselected, false);
        }

        private void ToggleSelection(int selected, bool isLit)
        {
            ref var battleSelectedComponent = ref _battleSelectedPool.Get(selected);
            if (battleSelectedComponent.SelectionType != BattleSelectionType.Unit)
                return;

            if (!TryGetUnitDiceModel(selected, out var diceModel))
                return;

            diceModel.IsLit.Update(isLit);
        }

        private void UpdateDices()
        {
            if (_battleAddedEventFilter.GetEntitiesCount() == 0)
                return;

            var areDicesAdded = false;
            foreach (var unit in _unitFilter)
            {
                var team = _teamService.GetTeam(unit);

                if (!_battleUIModel.DiceModels.ContainsKey(team))
                {
                    LogService.LogDebug(DebugType.Error, $"No models for dices of team {team}");
                    continue;
                }

                if (!_unitService.TryGetDice(unit, out var dice))
                {
                    LogService.LogDebug(DebugType.Error, $"No dice for unit {dice}");
                    continue;
                }

                ref var unitComponent = ref _unitPool.Get(unit);
                if (!_battleUIModel.DiceModels[team].TryGetValue(unitComponent.Id, out var diceModel))
                {
                    diceModel = new BattleDiceUIModel();
                    _battleUIModel.DiceModels[team].Add(unitComponent.Id, diceModel);
                    areDicesAdded = true;
                }

                diceModel.IsVisible.Update(!_battleDeathService.IsDead(unit));
                diceModel.DiceSide.UpdateEquatable(_battleDiceService.GetCurrentSide(dice));
                diceModel.IsInteractable.Update(false);
                diceModel.IsLocked.Update(_diceLockService.IsLocked(dice));
                diceModel.IsDimmed.Update(IsDimmed(dice));
            }

            if (areDicesAdded)
                _battleUIModel.AreDicesAdded.Invoke();
        }

        private void UpdateRolls()
        {
            foreach (var rollEvent in _dicesRollEventFilter)
            {
                ref var teamBattleDicesRollComponent = ref _teamBattleDicesRollPool.Get(rollEvent);

                var team = _teamService.GetTeam(rollEvent);
                UpdateTeamDices(team, teamBattleDicesRollComponent.RollsLeft > 0);
                UpdateRolls(team, teamBattleDicesRollComponent.RollsLeft);
            }
        }

        private void UpdateLock()
        {
            foreach (var dice in _diceLockAddedEventFilter)
                UpdateDiceLock(dice);
            foreach (var dice in _diceLockDeletedEventFilter)
                UpdateDiceLock(dice);
        }

        private void UpdateDimming()
        {
            if (_currentTeamModifiedEventFilter.GetEntitiesCount() > 0)
                foreach (var dice in _diceFilter)
                    if (TryGetDiceModel(dice, out var model))
                    {
                        model.IsDimmed.Update(IsDimmed(dice));
                        if (!_teamService.IsCurrentTeamEntity(dice))
                            model.IsInteractable.Update(false);
                    }

            foreach (var dice in _diceAimingEventFilter)
                if (TryGetDiceModel(dice, out var model))
                    model.IsDimmed.Update(IsDimmed(dice));
            foreach (var dice in _diceUnaimingEventFilter)
                if (TryGetDiceModel(dice, out var model))
                    model.IsDimmed.Update(IsDimmed(dice));
        }

        private void UpdateVisible()
        {
            foreach (var dice in _diceDeadAddedEventFilter)
            {
                if (!TryGetDiceModel(dice, out var diceModel))
                    continue;

                diceModel.IsVisible.Update(false);
            }
        }

        private void UpdateTeamDices(TeamType team, bool hasRolls)
        {
            foreach (var unit in _unitFilter)
            {
                if (!_teamService.IsTeamEntity(unit, team))
                    continue;

                if (!_battleUIModel.DiceModels.ContainsKey(team))
                {
                    LogService.LogDebug(DebugType.Error, $"No models for dices of team {team}");
                    continue;
                }

                if (!_unitService.TryGetDice(unit, out var dice))
                {
                    LogService.LogDebug(DebugType.Error, $"No dice for unit {dice}");
                    continue;
                }

                if (!TryGetDiceModel(dice, out var diceModel))
                    continue;

                diceModel.DiceSide.UpdateEquatable(_battleDiceService.GetCurrentSide(dice));
                diceModel.IsInteractable.Update(hasRolls);
            }
        }

        private void UpdateRolls(TeamType team, int rollsLeft)
        {
            if (!_battleUIModel.DiceRolls.TryGetValue(team, out var rolls))
            {
                LogService.LogDebug(DebugType.Error, $"No property for rolls of team {team}");
                return;
            }

            rolls.Update(rollsLeft);
        }

        private void UpdateDiceLock(int dice)
        {
            if (!TryGetDiceModel(dice, out var diceModel))
                return;

            diceModel.IsLocked.Update(_diceLockService.IsLocked(dice));
            _battleUIModel.AreAllTeamDicesLocked.Update(_diceLockService.AreCurrentTeamDicesLocked());
        }

        private bool TryGetDiceModel(int dice, out BattleDiceUIModel diceModel)
        {
            if (_battleDiceService.TryGetOwner(dice, out var owner))
                return TryGetUnitDiceModel(owner, out diceModel);

            LogService.LogDebug(DebugType.Error, $"No owner for dice {dice}");
            diceModel = null;
            return false;
        }

        private bool TryGetUnitDiceModel(int unit, out BattleDiceUIModel diceModel)
        {
            ref var unitComponent = ref _unitPool.Get(unit);
            if (_battleUIModel.DiceModels[_teamService.GetTeam(unit)].TryGetValue(unitComponent.Id, out diceModel))
                return true;

            LogService.LogDebug(DebugType.Error, $"No model for dice of unit {unitComponent.Id}");
            return false;
        }

        private bool IsDimmed(int dice)
            => _diceApplyService.IsApplyingDice || !_teamService.IsCurrentTeamEntity(dice) || _targetSelectedPool.Has(dice) ||
                _diceAimingViewPool.Has(dice);
    }
}