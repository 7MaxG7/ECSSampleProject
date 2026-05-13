using Infrastructure;
using Leopotam.EcsLite;
using Utils.Extensions;
using Zenject;

namespace Battle
{
    public class DamageService
    {
        private readonly EcsFilter _damagedFilter;
        private readonly EcsPool<DamagedComponent> _damagedPool;

        [Inject]
        public DamageService(EcsService ecsService)
        {
            _damagedFilter = ecsService.World.Filter<DamagedComponent>().End();
            _damagedPool = ecsService.World.GetPool<DamagedComponent>();
        }
        
        public void Damage(int targeted, int damage)
        {
            ref var damagedComponent = ref _damagedPool.GetOrAdd(targeted);
            damagedComponent.Damage = damage;
        }

        public bool IsDamaging(int target)
            => _damagedPool.Has(target);

        public bool IsDamagingAnyone()
            => _damagedFilter.GetEntitiesCount() > 0;
    }
}