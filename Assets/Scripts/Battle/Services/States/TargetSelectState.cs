using Abstractions.Battle;
using CustomTypes.Enums.Infrastructure;
using Dices;
using Infrastructure;

namespace Battle
{
    public class TargetSelectState : IBattleState
    {
        private readonly DiceTargetSelectService _targetSelectService;

        public TargetSelectState(DiceTargetSelectService targetSelectService)
        {
            _targetSelectService = targetSelectService;
        }
        
        public void Enter()
        {
            LogService.LogDebug(DebugType.Log, $"{nameof(TargetSelectState)} started");
            _targetSelectService.StartTargetSelection();
        }

        public void Exit()
        {
            _targetSelectService.FinishTargetSelection();
        }
    }
}