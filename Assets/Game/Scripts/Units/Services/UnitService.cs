using Infrastructure;
using Leopotam.EcsLite;
using Zenject;

namespace Units
{
    public class UnitService
    {
        private readonly EcsService _ecsService;
        private readonly EcsPool<UnitComponent> _unitPool;

        [Inject]
        public UnitService(EcsService ecsService)
        {
            _ecsService = ecsService;
            _unitPool = ecsService.World.GetPool<UnitComponent>();
        }
        
        public bool TryGetMainDice(int unit, out int dice)
        {
            ref var unitComponent = ref _unitPool.Get(unit);
            return _ecsService.TryUnpack(unitComponent.MainDice, out dice);
        }
    }
}