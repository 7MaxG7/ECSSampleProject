using Abstractions.Battle;
using CustomTypes.Enums.Infrastructure;
using Infrastructure;
using UI.Battle;
using Zenject;

namespace Battle
{
    public class EndBattleState : IBattleState
    {
        private readonly BattleDeathService _battleDeathService;
        private readonly BattleUIController _battleUIController;

        [Inject]
        public EndBattleState(BattleDeathService battleDeathService, BattleUIController battleUIController)
        {
            _battleDeathService = battleDeathService;
            _battleUIController = battleUIController;
        }

        public void Enter()
        {
            LogService.LogDebug(DebugType.Log, $"{nameof(EndBattleState)} started");

            var winner = _battleDeathService.GetAnyAliveTeam();
            _battleUIController.ShowBattleEndLabel(winner);
        }

        public void Exit()
        {
        }
    }
}