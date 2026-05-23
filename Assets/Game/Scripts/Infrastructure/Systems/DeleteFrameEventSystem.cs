using Leopotam.EcsLite;

namespace Infrastructure
{
    public class DeleteFrameEventSystem : IEcsPostRunSystem
    {
        private readonly FrameComponentsService _frameComponentsService;

        protected DeleteFrameEventSystem(FrameComponentsService frameComponentsService)
        {
            _frameComponentsService = frameComponentsService;
        }

        public void PostRun(IEcsSystems systems)
        {
            _frameComponentsService.ClearFrameComponents();
        }
    }
}