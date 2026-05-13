using Abstractions.Infrastructure;
using Leopotam.EcsLite;

namespace Infrastructure
{
    public class DeleteFrameEventSystem<T> : IEcsPostRunSystem where T : struct, IFrameEvent
    {
        private readonly EcsFilter _filter;
        private readonly EcsPool<T> _pool;

        public DeleteFrameEventSystem(EcsService ecsService)
        {
            _filter = ecsService.World.Filter<T>().End();
            _pool = ecsService.World.GetPool<T>();
        }
        
        public void PostRun(IEcsSystems systems)
        {
            foreach (var entity in _filter)
                _pool.Del(entity);
        }
    }
}