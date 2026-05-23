using Abstractions;
using Zenject;

namespace Battle
{
    public class BattleDiceRollState : IBattleState
    {
        private readonly BattleDiceRollService _battleDiceRollService;

        [Inject]
        public BattleDiceRollState(BattleDiceRollService battleDiceRollService)
        {
            _battleDiceRollService = battleDiceRollService;
        }

        public void Enter()
        {
            _battleDiceRollService.StartDiceRolling();
        }

        public void Exit()
        {
            _battleDiceRollService.FinishDiceRolling();
        }
    }
}