using System.Collections.Generic;
using Battle;
using CustomTypes;
using Dices;
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
        private readonly EcsFilter _dicesRollAddedEventFilter;
        private readonly EcsFilter _dicesRollDeletedEventFilter;
        private readonly EcsPool<TeamComponent> _teamPool;
        private readonly EcsPool<TeamBattleDicesRollComponent> _teamBattleDicesRollPool;

        [Inject]
        public BattleUIUpdateSystem(EcsService ecsService, DiceViewService diceViewService, BattleUIController battleUIController,
            BattleDiceLockService diceLockService, BattleDiceService battleDiceService, FrameComponentsService frameComponentsService)
        {
            _diceViewService = diceViewService;
            _battleUIController = battleUIController;
            _diceLockService = diceLockService;
            _battleDiceService = battleDiceService;

            _mainDicesFilter = ecsService.World.Filter<DiceComponent>().End();
            _dicesRollEventFilter = frameComponentsService.GetEventFilter<DicesRollEventComponent>();
            _dicesRollAddedEventFilter = frameComponentsService.GetAddedEventFilter<LockedComponent>();
            _dicesRollDeletedEventFilter = frameComponentsService.GetDeletedEventFilter<LockedComponent>();
            _teamPool = ecsService.World.GetPool<TeamComponent>();
            _teamBattleDicesRollPool = ecsService.World.GetPool<TeamBattleDicesRollComponent>();
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
                _battleUIController.SetRollsCountLabel(rollTeamComponent.Team, teamBattleDicesRollComponent.RollsLeft);
            }
        }

        private void UpdateDiceLockUI()
        {
            foreach (var diceLockEvent in _dicesRollAddedEventFilter)
                _diceViewService.ToggleDiceUILock(diceLockEvent, true);
            foreach (var diceLockEvent in _dicesRollDeletedEventFilter)
                _diceViewService.ToggleDiceUILock(diceLockEvent, false);

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