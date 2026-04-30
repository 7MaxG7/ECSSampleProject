using CustomTypes.Enums.Battle;
using CustomTypes.Enums.Infrastructure;
using Infrastructure;
using Leopotam.EcsLite;
using Units;
using Zenject;

namespace Battle
{
    public class BattleSelectionSystem : IEcsRunSystem
    {
        private readonly BattleUnitSelectionService _unitSelectionService;
        
        private readonly EcsFilter _battleSelectEventFilter;
        private readonly EcsFilter _battleDeselectEventFilter;
        private readonly EcsPool<BattleSelectedComponent> _battleSelectedPool;

        [Inject]
        public BattleSelectionSystem(EcsService ecsService, BattleUnitSelectionService unitSelectionService)
        {
            _unitSelectionService = unitSelectionService;
            
            _battleSelectEventFilter = ecsService.World.Filter<BattleSelectEventComponent>().End();
            _battleDeselectEventFilter = ecsService.World.Filter<BattleDeselectEventComponent>().End();
            _battleSelectedPool = ecsService.World.GetPool<BattleSelectedComponent>();
        }
        
        public void Run(IEcsSystems systems)
        {
            Deselect();
            Select();
        }

        private void Deselect()
        {
            foreach (var deselected in _battleDeselectEventFilter)
            {
                ToggleSelection(deselected, false);
                _battleSelectedPool.Del(deselected);
            }
        }

        private void Select()
        {
            foreach (var selected in _battleSelectEventFilter)
                ToggleSelection(selected, true);
        }

        private void ToggleSelection(int selected, bool mustSelected)
        {
            ref var battleSelectedComponent = ref _battleSelectedPool.Get(selected);
            switch (battleSelectedComponent.SelectionType)
            {
                case BattleSelectionType.Unit:
                    _unitSelectionService.ToggleSelection(selected, mustSelected);
                    break;
                default:
                    LogService.LogDebug(DebugType.Warning, $"Cannot get service to select {selected} of type {battleSelectedComponent.SelectionType}");
                    return;
            }
        }
    }
}