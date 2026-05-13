using CustomTypes.Enums.Battle;
using CustomTypes.Enums.Infrastructure;
using Infrastructure;
using Leopotam.EcsLite;
using Units;
using Zenject;

namespace Battle
{
    public class BattleSelectionViewSystem : IEcsRunSystem
    {
        private readonly UnitViewService _unitViewService;

        private readonly EcsFilter _battleSelectEventFilter;
        private readonly EcsFilter _battleDeselectEventFilter;
        private readonly EcsPool<BattleSelectEventComponent> _battleSelectEventPool;
        private readonly EcsPool<BattleDeselectEventComponent> _battleDeselectEventPool;

        [Inject]
        public BattleSelectionViewSystem(EcsService ecsService, UnitViewService unitViewService)
        {
            _unitViewService = unitViewService;

            _battleSelectEventFilter = ecsService.World.Filter<BattleSelectEventComponent>().End();
            _battleDeselectEventFilter = ecsService.World.Filter<BattleDeselectEventComponent>().End();
            _battleSelectEventPool = ecsService.World.GetPool<BattleSelectEventComponent>();
            _battleDeselectEventPool = ecsService.World.GetPool<BattleDeselectEventComponent>();
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
                ref var battleDeselectEventComponent = ref _battleDeselectEventPool.Get(deselected);
                ToggleSelection(deselected, battleDeselectEventComponent.SelectionType, false);
            }
        }

        private void Select()
        {
            foreach (var selected in _battleSelectEventFilter)
            {
                ref var battleSelectEventComponent = ref _battleSelectEventPool.Get(selected);
                ToggleSelection(selected, battleSelectEventComponent.SelectionType, true);
            }
        }

        private void ToggleSelection(int selected, BattleSelectionType selectionType, bool mustSelected)
        {
            switch (selectionType)
            {
                case BattleSelectionType.Unit:
                    _unitViewService.ToggleUnitDiceHighlight(selected, mustSelected);
                    break;
                
                default:
                    LogService.LogDebug(DebugType.Warning, $"Cannot get service to select {selected} of type {selectionType}");
                    return;
            }
        }
    }
}