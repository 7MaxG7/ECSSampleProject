using Abstractions.Battle;
using UI.Battle;

namespace Battle
{
    public class BattleDiceRollState : IBattleState
    {
        private readonly BattleDiceRollService _rollService;
        private readonly BattleUIController _battleUIController;

        public BattleDiceRollState(BattleDiceRollService rollService, BattleUIController battleUIController)
        {
            _rollService = rollService;
            _battleUIController = battleUIController;
        }

        public void Enter()
        {
            _rollService.StartDiceRolling();
            _battleUIController.ToggleRollUIInteractable(true);
        }

        public void Exit()
        {
            _battleUIController.ToggleRollUIInteractable(false);
            _rollService.FinishDiceRolling();
        }
    }
}