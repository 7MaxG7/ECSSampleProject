using Battle;
using Leopotam.EcsLite;
using Utils;
using Zenject;

namespace UI.Battle
{
    public class BattleUpdateUISystem : IEcsPostRunSystem
    {
        private readonly BattleUIModel _battleUIModel;
        private readonly BattleDiceRollService _battleDiceRollService;
        private readonly BattleDeathService _battleDeathService;

        [Inject]
        public BattleUpdateUISystem(BattleUIModel battleUIModel, BattleDiceRollService battleDiceRollService,
            BattleDeathService battleDeathService)
        {
            _battleUIModel = battleUIModel;
            _battleDiceRollService = battleDiceRollService;
            _battleDeathService = battleDeathService;
        }

        public void PostRun(IEcsSystems systems)
        {
            UpdateRollUI();
            UpdateBattleEndUI();
        }

        private void UpdateRollUI()
        {
            _battleUIModel.IsRollUIInteractable.Update(_battleDiceRollService.IsRollingState);
        }

        private void UpdateBattleEndUI()
        {
            var isBattleEnded = _battleDeathService.IsAnyTeamDead();
            _battleUIModel.IsBattleEndUIVisible.Update(isBattleEnded);
            if (!isBattleEnded)
                return;
            
            var winner = _battleDeathService.GetAnyAliveTeam();
            _battleUIModel.Winner.Update(winner);
        }
    }
}