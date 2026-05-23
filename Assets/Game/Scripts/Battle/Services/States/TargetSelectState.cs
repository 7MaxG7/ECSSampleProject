using Abstractions;
using CustomTypes;
using Dices;
using Infrastructure;
using Zenject;

namespace Battle
{
    public class TargetSelectState : IBattleState
    {
        private readonly DiceTargetSelectService _targetSelectService;

        [Inject]
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