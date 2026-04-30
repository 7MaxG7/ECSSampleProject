using System.Collections.Generic;
using Battle;
using CustomTypes;
using CustomTypes.Enums.Infrastructure;
using CustomTypes.Enums.Team;
using Dices;
using Dices.Events;
using Infrastructure;
using Leopotam.EcsLite;
using UI.Battle;
using Zenject;

namespace UI
{
    public class BattleUIUpdateSystem : IEcsPostRunSystem
    {
        private readonly DiceViewService _diceViewService;
        private readonly BattleUIController _battleUIController;
        private readonly BattleDiceLockService _diceLockService;
        private readonly BattleDiceService _battleDiceService;

        private readonly EcsFilter _mainDicesFilter;
        private readonly EcsFilter _dicesRollEventFilter;
        private readonly EcsFilter _diceLockEventFilter;
        private readonly EcsPool<TeamComponent> _teamPool;
        private readonly EcsPool<TeamBattleDicesRollComponent> _teamBattleDicesRollPool;
        private readonly EcsPool<DiceLockEventComponent> _diceLockEventPool;

        [Inject]
        public BattleUIUpdateSystem(EcsService ecsService, DiceViewService diceViewService, BattleUIController battleUIController,
            BattleDiceLockService diceLockService, BattleDiceService battleDiceService)
        {
            _diceViewService = diceViewService;
            _battleUIController = battleUIController;
            _diceLockService = diceLockService;
            _battleDiceService = battleDiceService;

            _mainDicesFilter = ecsService.World.Filter<DiceComponent>().End();
            _dicesRollEventFilter = ecsService.World.Filter<DicesRollEventComponent>().End();
            _diceLockEventFilter = ecsService.World.Filter<DiceLockEventComponent>().End();
            _teamPool = ecsService.World.GetPool<TeamComponent>();
            _teamBattleDicesRollPool = ecsService.World.GetPool<TeamBattleDicesRollComponent>();
            _diceLockEventPool = ecsService.World.GetPool<DiceLockEventComponent>();
        }

        public void PostRun(IEcsSystems systems)
        {
            UpdateDiceSidesUI();
            UpdateDiceLockUI();
        }

        private void UpdateDiceSidesUI()
        {
            foreach (var rollEvent in _dicesRollEventFilter)
            {
                ref var rollTeamComponent = ref _teamPool.Get(rollEvent);
                ref var teamBattleDicesRollComponent = ref _teamBattleDicesRollPool.Get(rollEvent);

                var dices = CreateTeamDiceDatas(rollTeamComponent.Team, teamBattleDicesRollComponent.RollsLeft);
                _battleUIController.ShowCurrentDices(dices, rollTeamComponent.Team);
                _battleUIController.UpdateRollsCountLabel(rollTeamComponent.Team, teamBattleDicesRollComponent.RollsLeft);
            }
        }

        private void UpdateDiceLockUI()
        {
            foreach (var diceLockEvent in _diceLockEventFilter)
            {
                ref var diceLockEventComponent = ref _diceLockEventPool.Get(diceLockEvent);
                _diceViewService.ToggleDiceUILock(diceLockEvent, diceLockEventComponent.IsLocked);
            }

            _battleUIController.UpdateRollButtonLabel(_diceLockService.AreCurrentTeamDicesLocked());
        }

        private List<DiceData> CreateTeamDiceDatas(TeamType team, int rollsLeft)
        {
            var dices = new List<DiceData>();
            foreach (var dice in _mainDicesFilter)
            {
                ref var diceTeamComponent = ref _teamPool.Get(dice);
                if (diceTeamComponent.Team != team)
                    continue;

                if (!TryCreateDiceData(dice, rollsLeft > 0, out var diceData))
                    continue;

                dices.Add(diceData);
            }

            return dices;
        }

        private bool TryCreateDiceData(int dice, bool mustInteractable, out DiceData diceData)
        {
            if (!_battleDiceService.TryGetUnit(dice, out var unit))
            {
                LogService.LogDebug(DebugType.Error, $"No owner for dice {dice}");
                diceData = null;
                return false;
            }

            diceData = new DiceData(_battleDiceService.GetCurrentSide(dice), unit, mustInteractable);
            return true;
        }
    }
}