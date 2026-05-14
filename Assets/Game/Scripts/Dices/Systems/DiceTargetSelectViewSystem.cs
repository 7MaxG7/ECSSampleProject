using Battle;
using CustomTypes;
using Cysharp.Threading.Tasks;
using Infrastructure;
using Leopotam.EcsLite;
using UI.Units;
using Zenject;

namespace Dices
{
    public class DiceTargetSelectViewSystem : IEcsPostRunSystem
    {
        private readonly EcsService _ecsService;
        private readonly InputService _inputService;
        private readonly HighlightService _highlightService;
        private readonly DiceTargetSelectService _diceTargetSelectService;
        private readonly DiceViewService _diceViewService;
        private readonly UnitOverlayUIService _unitOverlayUIService;
        private readonly BattleDiceService _battleDiceService;

        private readonly EcsFilter _diceTargetingFilter;
        private readonly EcsFilter _aimedFilter;
        private readonly EcsFilter _unaimedFilter;
        private readonly EcsFilter _diceAimingEventFilter;
        private readonly EcsFilter _unaimingEventFilter;
        private readonly EcsFilter _targetSelectedAddedEventFilter;
        private readonly EcsPool<DiceAimingViewComponent> _diceAimingViewPool;
        private readonly EcsPool<AimedWithDiceComponent> _aimedWithDicePool;
        private readonly EcsPool<UnaimedWithDiceComponent> _unaimedWithDicePool;
        private readonly EcsPool<TargetSelectedComponent> _targetSelectedPool;

        [Inject]
        public DiceTargetSelectViewSystem(EcsService ecsService, InputService inputService, DiceTargetSelectService diceTargetSelectService,
            HighlightService highlightService, DiceViewService diceViewService, UnitOverlayUIService unitOverlayUIService,
            BattleDiceService battleDiceService, FrameComponentsService frameComponentsService)
        {
            _ecsService = ecsService;
            _inputService = inputService;
            _highlightService = highlightService;
            _diceTargetSelectService = diceTargetSelectService;
            _diceViewService = diceViewService;
            _unitOverlayUIService = unitOverlayUIService;
            _battleDiceService = battleDiceService;

            _unaimingEventFilter = frameComponentsService.GetEventFilter<DiceUnaimingEventComponent>();
            _diceAimingEventFilter = frameComponentsService.GetEventFilter<DiceAimingEventComponent>();
            _targetSelectedAddedEventFilter = frameComponentsService.GetAddedEventFilter<TargetSelectedComponent>();
            _diceTargetingFilter = ecsService.World.Filter<DiceAimingViewComponent>().End();
            _aimedFilter = ecsService.World.Filter<AimedWithDiceComponent>().Exc<UnaimedWithDiceComponent>().End();
            _unaimedFilter = ecsService.World.Filter<UnaimedWithDiceComponent>().End();
            _diceAimingViewPool = ecsService.World.GetPool<DiceAimingViewComponent>();
            _aimedWithDicePool = ecsService.World.GetPool<AimedWithDiceComponent>();
            _unaimedWithDicePool = ecsService.World.GetPool<UnaimedWithDiceComponent>();
            _targetSelectedPool = ecsService.World.GetPool<TargetSelectedComponent>();
        }

        public void PostRun(IEcsSystems systems)
        {
            if (!_diceTargetSelectService.IsTargetSelecting)
                return;

            MarkAimedForHighlightDisable();
            UpdateAimingDiceViews();
            UpdateAiming();
            DisableHighlightForUnaimed();
            AddOverlayDiceFacet();
        }

        private void MarkAimedForHighlightDisable()
        {
            foreach (var aimed in _aimedFilter)
                _unaimedWithDicePool.Add(aimed);
        }

        private void UpdateAimingDiceViews()
        {
            foreach (var aimingEvent in _diceAimingEventFilter)
                _diceViewService.ToggleDiceHide(aimingEvent, true);

            foreach (var aimingEvent in _unaimingEventFilter)
                _diceViewService.ToggleDiceHide(aimingEvent, false);
        }

        private void UpdateAiming()
        {
            foreach (var aiming in _diceTargetingFilter)
            {
                UpdateAimPosition(aiming);
                UpdateAimedUnitHighlight(aiming);
            }
        }

        private void UpdateAimPosition(int aiming)
        {
            ref var diceAimingComponent = ref _diceAimingViewPool.Get(aiming);
            diceAimingComponent.DiceAimView.transform.position = _inputService.MousePosition;
        }

        private void UpdateAimedUnitHighlight(int aiming)
        {
            if (!_diceTargetSelectService.IsCurrentTargetValid(aiming, out var target))
                return;

            if (_aimedWithDicePool.Has(target))
                _unaimedWithDicePool.Del(target);
            else
            {
                _aimedWithDicePool.Add(target);
                _highlightService.SetHighlight(target, true, HighlightType.Aiming);
            }
        }

        private void DisableHighlightForUnaimed()
        {
            foreach (var unaimed in _unaimedFilter)
            {
                _aimedWithDicePool.Del(unaimed);
                _unaimedWithDicePool.Del(unaimed);
                _highlightService.SetHighlight(unaimed, false);
            }
        }

        private void AddOverlayDiceFacet()
        {
            foreach (var diceTargetedEvent in _targetSelectedAddedEventFilter)
            {
                ref var targetSelectedComponent = ref _targetSelectedPool.Get(diceTargetedEvent);
                if (!_ecsService.TryUnpack(targetSelectedComponent.Target, out var target))
                    continue;

                // TODO. Call overlayUIController, which will add facet to model
                AddOverlayDiceFacetAsync(target, diceTargetedEvent).Forget();
            }
        }

        private async UniTaskVoid AddOverlayDiceFacetAsync(int target, int dice)
        {
            var facetIcon = await _unitOverlayUIService.AddOverlayDiceFacetAsync(target, _battleDiceService.GetCurrentSide(dice));
            _diceViewService.SetDiceFacetIcon(dice, facetIcon);
        }
    }
}