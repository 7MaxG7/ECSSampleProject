using Infrastructure;
using Leopotam.EcsLite;
using Zenject;

namespace Battle
{
    public class DamageSystem : IEcsRunSystem
    {
        private readonly HealthService _healthService;

        private readonly EcsFilter _damagedFilter;
        private readonly EcsPool<DamagedComponent> _damagedPool;
        private readonly EcsPool<DeadComponent> _deadPool;

        [Inject]
        public DamageSystem(EcsService ecsService, HealthService healthService)
        {
            _healthService = healthService;

            _damagedFilter = ecsService.World.Filter<DamagedComponent>().Exc<DeadComponent>().End();
            _damagedPool = ecsService.World.GetPool<DamagedComponent>();
        }
        
        public void Run(IEcsSystems systems)
        {
            foreach (var damaged in _damagedFilter)
            {
                ref var damagedComponent = ref _damagedPool.Get(damaged);

                _healthService.TryReduceArmor(damaged, damagedComponent.Damage, out var damageLeft);
                
                if (damageLeft > 0)
                    _healthService.ReduceHealth(damaged, damageLeft);
                
                _damagedPool.Del(damaged);
            }
        }
    }
}