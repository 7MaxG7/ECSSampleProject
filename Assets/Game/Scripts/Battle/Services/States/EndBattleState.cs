using Abstractions;
using CustomTypes;
using Infrastructure;
using Zenject;

namespace Battle
{
    public class EndBattleState : IBattleState
    {
        [Inject]
        public EndBattleState()
        {
        }

        public void Enter()
        {
            LogService.LogDebug(DebugType.Log, $"{nameof(EndBattleState)} started");
        }

        public void Exit()
        {
        }
    }
}