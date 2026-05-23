using Battle;
using CustomTypes;
using Infrastructure;
using Leopotam.EcsLite;
using Zenject;

namespace Dices
{
    public class DiceTargetSelectViewSystem : IEcsPostRunSystem
    {
        private readonly InputService _inputService;
        private readonly HighlightService _highlightService;
        private readonly DiceTargetSelectService _diceTargetSelectService;

        private readonly EcsFilter _diceTargetingFilter;
        private readonly EcsFilter _aimedFilter;
        private readonly EcsFilter _unaimedFilter;
        private readonly EcsPool<DiceAimingViewComponent> _diceAimingViewPool;
        private readonly EcsPool<AimedWithDiceComponent> _aimedWithDicePool;
        private readonly EcsPool<UnaimedWithDiceComponent> _unaimedWithDicePool;

        [Inject]
        public DiceTargetSelectViewSystem(EcsService ecsService, InputService inputService, DiceTargetSelectService diceTargetSelectService,
            HighlightService highlightService, FrameComponentsService frameComponentsService)
        {
            _inputService = inputService;
            _highlightService = highlightService;
            _diceTargetSelectService = diceTargetSelectService;

            _diceTargetingFilter = ecsService.World.Filter<DiceAimingViewComponent>().End();
            _aimedFilter = ecsService.World.Filter<AimedWithDiceComponent>().Exc<UnaimedWithDiceComponent>().End();
            _unaimedFilter = ecsService.World.Filter<UnaimedWithDiceComponent>().End();
            _diceAimingViewPool = ecsService.World.GetPool<DiceAimingViewComponent>();
            _aimedWithDicePool = ecsService.World.GetPool<AimedWithDiceComponent>();
            _unaimedWithDicePool = ecsService.World.GetPool<UnaimedWithDiceComponent>();
        }

        public void PostRun(IEcsSystems systems)
        {
            if (!_diceTargetSelectService.IsTargetSelecting)
                return;

            MarkAimedForHighlightDisable();
            UpdateAiming();
            DisableHighlightForUnaimed();
        }

        private void MarkAimedForHighlightDisable()
        {
            foreach (var aimed in _aimedFilter)
                _unaimedWithDicePool.Add(aimed);
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
    }
}