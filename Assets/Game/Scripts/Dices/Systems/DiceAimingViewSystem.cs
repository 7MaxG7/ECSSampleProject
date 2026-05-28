using Battle;
using CustomTypes;
using Infrastructure;
using Leopotam.EcsLite;
using Zenject;

namespace Dices
{
    public class DiceAimingViewSystem : IEcsPostRunSystem
    {
        private readonly EcsService _ecsService;
        private readonly HighlightService _highlightService;
        private readonly DiceTargetSelectService _diceTargetSelectService;

        private readonly EcsFilter _aimingDiceFilter;
        private readonly EcsFilter _aimingDiceDeleteEventFilter;
        
        private EcsPackedEntity? _lastAimedTarget;

        [Inject]
        public DiceAimingViewSystem(EcsService ecsService, DiceTargetSelectService diceTargetSelectService,
            HighlightService highlightService, FrameComponentsService frameComponentsService)
        {
            _ecsService = ecsService;
            _highlightService = highlightService;
            _diceTargetSelectService = diceTargetSelectService;

            _aimingDiceFilter = ecsService.World.Filter<AimingDiceComponent>().End();
            _aimingDiceDeleteEventFilter = frameComponentsService.GetDeletedEventFilter<AimingDiceComponent>();
        }

        public void PostRun(IEcsSystems systems)
        {
            if (!_diceTargetSelectService.IsTargetSelecting)
                return;

            UpdateUnitsHighlight();
        }

        private void UpdateUnitsHighlight()
        {
            foreach (var dice in _aimingDiceFilter)
                UpdateAimedUnitHighlight(dice);

            if (_aimingDiceDeleteEventFilter.GetEntitiesCount() > 0 && _ecsService.TryUnpack(_lastAimedTarget, out var lastTarget))
            {
                _highlightService.SetHighlight(lastTarget, false);
                _lastAimedTarget = null;
            }
        }

        private void UpdateAimedUnitHighlight(int aiming)
        {
            var isAiming = _diceTargetSelectService.IsCurrentTargetValid(aiming, out var target);
            var hadAiming = _ecsService.TryUnpack(_lastAimedTarget, out var lastTarget);

            if (hadAiming && (!isAiming || target != lastTarget))
            {
                _highlightService.SetHighlight(lastTarget, false);
                _lastAimedTarget = null;
            }

            if (isAiming && (!hadAiming || target != lastTarget))
            {
                _highlightService.SetHighlight(target, true, HighlightType.Aiming);
                _lastAimedTarget = _ecsService.World.PackEntity(target);
            }
        }
    }
}