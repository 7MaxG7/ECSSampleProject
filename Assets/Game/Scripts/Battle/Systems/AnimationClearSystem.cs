using Leopotam.EcsLite;
using Units;
using Zenject;

namespace Battle
{
    public class AnimationClearSystem : IEcsPostRunSystem
    {
        private readonly BattleAnimatorService _battleAnimatorService;

        [Inject]
        public AnimationClearSystem(BattleAnimatorService battleAnimatorService)
        {
            _battleAnimatorService = battleAnimatorService;
        }
        
        public void PostRun(IEcsSystems systems)
        {
            _battleAnimatorService.ClearReadyAnimationActions();
        }
    }
}