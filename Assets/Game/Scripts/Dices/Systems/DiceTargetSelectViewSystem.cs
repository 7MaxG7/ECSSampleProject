using Battle;
using CustomTypes.Enums.Battle;
using Cysharp.Threading.Tasks;
using Dices.Events;
using Infrastructure;
using Infrastructure.Input;
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
        private readonly BattleStateMachine _battleStateMachine;

        private readonly EcsFilter _diceTargetingFilter;
        private readonly EcsFilter _aimedFilter;
        private readonly EcsFilter _unaimedFilter;
        private readonly EcsFilter _aimingEventFilter;
        private readonly EcsFilter _unaimingEventFilter;
        private readonly EcsFilter _diceTargetedEventFilter;
        private readonly EcsPool<DiceAimingViewComponent> _diceAimingViewPool;
        private readonly EcsPool<AimedWithDiceComponent> _aimedWithDicePool;
        private readonly EcsPool<UnaimedWithDiceComponent> _unaimedWithDicePool;
        private readonly EcsPool<TargetSelectedComponent> _targetSelectedPool;

        [Inject]
        public DiceTargetSelectViewSystem(EcsService ecsService, InputService inputService, DiceTargetSelectService diceTargetSelectService,
            HighlightService highlightService, DiceViewService diceViewService, UnitOverlayUIService unitOverlayUIService,
            BattleDiceService battleDiceService, BattleStateMachine battleStateMachine)
        {
            _ecsService = ecsService;
            _inputService = inputService;
            _highlightService = highlightService;
            _diceTargetSelectService = diceTargetSelectService;
            _diceViewService = diceViewService;
            _unitOverlayUIService = unitOverlayUIService;
            _battleDiceService = battleDiceService;
            _battleStateMachine = battleStateMachine;

            _diceTargetingFilter = ecsService.World.Filter<DiceAimingViewComponent>().End();
            _aimedFilter = ecsService.World.Filter<AimedWithDiceComponent>().Exc<UnaimedWithDiceComponent>().End();
            _unaimedFilter = ecsService.World.Filter<UnaimedWithDiceComponent>().End();
            _aimingEventFilter = ecsService.World.Filter<DiceAimingEventComponent>().End();
            _unaimingEventFilter = ecsService.World.Filter<DiceUnaimingEventComponent>().End();
            _diceTargetedEventFilter = ecsService.World.Filter<DiceTargetedEventComponent>().End();
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
            foreach (var aimingEvent in _aimingEventFilter)
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
            foreach (var diceTargetedEvent in _diceTargetedEventFilter)
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