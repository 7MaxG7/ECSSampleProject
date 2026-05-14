using CustomTypes;
using Infrastructure;
using Leopotam.EcsLite;
using Units;
using Utils;
using Zenject;

namespace Battle
{
    public class UnitAnimationLaunchViewSystem : IEcsRunSystem
    {
        private readonly BattleDeathService _deathService;
        private readonly BattleAnimatorService _animatorService;

        private readonly EcsFilter _unitAnimationLaunchEventFilter;
        private readonly EcsPool<AnimationLaunchEventComponent> _animationLaunchPool;

        [Inject]
        public UnitAnimationLaunchViewSystem(EcsService ecsService, BattleDeathService deathService, BattleAnimatorService animatorService,
            FrameComponentsService frameComponentsService)
        {
            _deathService = deathService;
            _animatorService = animatorService;

            _unitAnimationLaunchEventFilter =
                frameComponentsService.GetEventMask<AnimationLaunchEventComponent>().Inc<UnitViewComponent>().End();
            _animationLaunchPool = ecsService.World.GetPool<AnimationLaunchEventComponent>();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var unit in _unitAnimationLaunchEventFilter)
            {
                ref var animationLaunchComponent = ref _animationLaunchPool.Get(unit);
                var sideType = animationLaunchComponent.DiceSideType;

                switch (animationLaunchComponent.AnimationType)
                {
                    case BattleAnimationType.FacetApply:
                        _animatorService.PlayFacetAnimation(unit, sideType);
                        break;

                    case BattleAnimationType.FacetReaction:
                        if (sideType.IsDamageSide() && _deathService.IsDead(unit))
                            _animatorService.ToggleDeathAnimation(unit, true);
                        else
                            _animatorService.PlayFacetReactionAnimation(unit, sideType);
                        break;
                }
            }
        }
    }
}