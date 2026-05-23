using Abstractions;
using CustomTypes;
using Infrastructure;
using Zenject;

namespace Battle
{
    public class BattleBeginState : IBattleState
    {
        private readonly BattleBeginService _battleBeginService;

        [Inject]
        public BattleBeginState(BattleBeginService battleBeginService)
        {
            _battleBeginService = battleBeginService;
        }

        public void Enter()
        {
            LogService.LogDebug(DebugType.Log, $"{nameof(BattleBeginState)} started");
            _battleBeginService.StartBattleBegin();
        }

        public void Exit()
        {
            _battleBeginService.FinishBattleBegin();
        }
    }
}