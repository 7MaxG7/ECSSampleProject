using Infrastructure;
using Leopotam.EcsLite;
using Zenject;

namespace Units
{
    public class UnitService
    {
        private readonly EcsService _ecsService;
        private readonly EcsFilter _unitFilter;
        private readonly EcsPool<UnitComponent> _unitPool;

        [Inject]
        public UnitService(EcsService ecsService)
        {
            _ecsService = ecsService;
            _unitFilter = ecsService.World.Filter<UnitComponent>().End();
            _unitPool = ecsService.World.GetPool<UnitComponent>();
        }
        
        public bool TryGetDice(int unit, out int dice)
        {
            ref var unitComponent = ref _unitPool.Get(unit);
            return _ecsService.TryUnpack(unitComponent.Dice, out dice);
        }

        public bool TryGetUnit(string id, out int resultUnit)
        {
            foreach (var unit in _unitFilter)
            {
                ref var unitComponent = ref _unitPool.Get(unit);
                if (!id.Equals(unitComponent.Id))
                    continue;
                
                resultUnit = unit;
                return true;
            }
            
            resultUnit = -1;
            return false;
        }
    }
}