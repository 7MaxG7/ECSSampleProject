using System;
using System.Threading;
using CustomTypes;
using Cysharp.Threading.Tasks;
using Infrastructure;
using Leopotam.EcsLite;
using Units;
using Zenject;

namespace Dices
{
    public class DiceApplyViewService
    {
        public bool IsInProgress { get; private set; }

        private readonly BattleAnimatorService _battleAnimatorService;
        private readonly BattleAnimationConfig _animationConfig;
        private readonly FrameComponentsService _frameComponentsService;

        private readonly EcsPool<ViewUpdateDelayComponent> _viewUpdateDelayPool;

        [Inject]
        public DiceApplyViewService(EcsService ecsService, BattleAnimatorService battleAnimatorService,
            BattleAnimationConfig animationConfig, FrameComponentsService frameComponentsService)
        {
            _battleAnimatorService = battleAnimatorService;
            _animationConfig = animationConfig;
            _frameComponentsService = frameComponentsService;

            _viewUpdateDelayPool = ecsService.World.GetPool<ViewUpdateDelayComponent>();
        }

        public async UniTask AnimateDiceApplyAsync(int unit, int targeted, DiceSideType sideType, CancellationTokenSource cts)
        {
            IsInProgress = true;

            await StartApplyAnimation(unit, targeted, sideType, cts);
            await EndApplyAnimation(targeted, sideType, cts);

            await UniTask.Delay(TimeSpan.FromSeconds(_animationConfig.BetweenUnitsApplyingDelay), cancellationToken: cts.Token);
            IsInProgress = false;
        }

        private async UniTask StartApplyAnimation(int unit, int targeted, DiceSideType sideType, CancellationTokenSource cts)
        {
            _viewUpdateDelayPool.Add(targeted);
            AddAnimationComponent(unit, sideType, BattleAnimationType.FacetApply);

            while (_battleAnimatorService.IsAnimationInProcess && !_battleAnimatorService.IsApplyActionReady(unit))
                await UniTask.NextFrame(cts.Token);
        }

        private async UniTask EndApplyAnimation(int targeted, DiceSideType sideType, CancellationTokenSource cts)
        {
            AnimateTarget(targeted, sideType);
            while (_battleAnimatorService.IsAnimationInProcess)
                await UniTask.NextFrame(cts.Token);
        }

        private void AnimateTarget(int targeted, DiceSideType sideType)
        {
            _viewUpdateDelayPool.Del(targeted);
            AddAnimationComponent(targeted, sideType, BattleAnimationType.FacetReaction);
        }

        private void AddAnimationComponent(int unit, DiceSideType sideType, BattleAnimationType animationType)
        {
            ref var animationLaunchEventComponent = ref _frameComponentsService.AddEvent<AnimationLaunchEventComponent>(unit);
            animationLaunchEventComponent.AnimationType = animationType;
            animationLaunchEventComponent.DiceSideType = sideType;
        }
    }
}