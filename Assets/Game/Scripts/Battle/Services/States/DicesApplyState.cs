using Abstractions;
using CustomTypes;
using Dices;
using Infrastructure;
using Zenject;

namespace Battle
{
    public class DicesApplyState : IBattleState
    {
        private readonly DiceApplyService _diceApplyService;

        [Inject]
        public DicesApplyState(DiceApplyService diceApplyService)
        {
            _diceApplyService = diceApplyService;
        }
        
        public void Enter()
        {
            LogService.LogDebug(DebugType.Log, $"{nameof(DicesApplyState)} started");
            _diceApplyService.StartDiceApplying();
        }

        public void Exit()
            => _diceApplyService.FinishDiceApplying();
    }
}