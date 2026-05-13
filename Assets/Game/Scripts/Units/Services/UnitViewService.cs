using Battle;
using Dices;
using Infrastructure;
using Leopotam.EcsLite;
using Zenject;

namespace Units
{
    public class UnitViewService
    {
        private readonly EcsService _ecsService;
        private readonly UnitService _unitService;
        private readonly DiceViewService _diceViewService;
        private readonly HighlightService _highlightService;

        private readonly EcsPool<UnitComponent> _unitPool;

        [Inject]
        public UnitViewService(EcsService ecsService, HighlightService highlightService, BattleAnimatorService battleAnimatorService,
            UnitService unitService, DiceViewService diceViewService)
        {
            _ecsService = ecsService;
            _unitService = unitService;
            _diceViewService = diceViewService;
            _highlightService = highlightService;

            _unitPool = ecsService.World.GetPool<UnitComponent>();
        }

        public void ToggleUnitDiceHighlight(int unit, bool mustHighlighted)
        {
            ref var unitComponent = ref _unitPool.Get(unit);
            if (!_ecsService.TryUnpackWithWarning(unitComponent.MainDice, out var dice))
                return;

            _highlightService.SetHighlight(dice, mustHighlighted);
        }

        public void Die(int unit)
        {
            if (_unitService.TryGetMainDice(unit, out var dice))
                _diceViewService.Die(dice);
        }
    }
}