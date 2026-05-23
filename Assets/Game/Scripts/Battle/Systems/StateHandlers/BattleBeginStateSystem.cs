using Leopotam.EcsLite;
using Zenject;

namespace Battle
{
    public class BattleBeginStateSystem : IEcsRunSystem
    {
        private readonly BattleStateMachine _battleStateMachine;
        private readonly BattleBeginService _battleBeginService;

        [Inject]
        public BattleBeginStateSystem(BattleStateMachine battleStateMachine, BattleBeginService battleBeginService)
        {
            _battleStateMachine = battleStateMachine;
            _battleBeginService = battleBeginService;
        }

        public void Run(IEcsSystems systems)
        {
            if (!_battleBeginService.IsBattleBeginState)
                return;
            
            _battleStateMachine.Enter<BattleDiceRollState>();
        }
    }
}