using CustomTypes;
using Infrastructure;
using Leopotam.EcsLite;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace Battle
{
    public class BattleSelectionService
    {
        private readonly EcsService _ecsService;
        private readonly SelectionConfig _selectionConfig;
        private readonly InputService _inputService;

        private readonly EcsFilter _selectedFilter;
        private readonly EcsPool<BattleSelectedComponent> _battleSelectedPool;
        private readonly EcsPool<BattleSelectEventComponent> _battleSelectEventPool;
        private readonly EcsPool<BattleDeselectEventComponent> _battleDeselectEventPool;

        private readonly RaycastHit[] _raycastHits;

        [Inject]
        public BattleSelectionService(EcsService ecsService, SelectionConfig selectionConfig, InputService inputService)
        {
            _ecsService = ecsService;
            _selectionConfig = selectionConfig;
            _inputService = inputService;

            _selectedFilter = ecsService.World.Filter<BattleSelectedComponent>().End();
            _battleSelectedPool = ecsService.World.GetPool<BattleSelectedComponent>();
            _battleSelectEventPool = ecsService.World.GetPool<BattleSelectEventComponent>();
            _battleDeselectEventPool = ecsService.World.GetPool<BattleDeselectEventComponent>();
            
            _raycastHits = new RaycastHit[_selectionConfig.SelectionRaycastHitsCount];
        }

        public void Init()
        {
            _inputService.UserInputControls.BattleSelection.InfoClick.performed += ShowInfo;
            _inputService.UserInputControls.BattleSelection.Enable();
        }

        public void OnDispose()
        {
            _inputService.UserInputControls.BattleSelection.Disable();
            _inputService.UserInputControls.BattleSelection.InfoClick.performed -= ShowInfo;
        }

        public bool TryGetSelection(out int selected, out BattleSelectionType selectionType)
        {
            selected = -1;
            selectionType = BattleSelectionType.None;
            return TryRaycastSelection(out var hitsCount) && TryIdentifySelection(hitsCount, out selected, out selectionType);
        }

        private void ShowInfo(InputAction.CallbackContext _)
        {
            ClearSelection();
            
            if (!TryGetSelection(out var selected, out var selectionType))
                return;

            MarkSelected(selected, selectionType);
        }

        private void ClearSelection()
        {
            foreach (var selected in _selectedFilter)
            {
                ref var battleDeselectEventComponent = ref _battleDeselectEventPool.Add(selected);
                ref var battleSelectedComponent = ref _battleSelectedPool.Get(selected);
                battleDeselectEventComponent.SelectionType = battleSelectedComponent.SelectionType;
                _battleSelectedPool.Del(selected);
            }
        }

        private bool TryRaycastSelection(out int hitsCount)
        {
            // ReSharper disable once PossibleNullReferenceException
            var ray = Camera.main.ScreenPointToRay(_inputService.MousePosition);
            hitsCount = Physics.RaycastNonAlloc(ray, _raycastHits, _selectionConfig.SelectionRaycastLength, _selectionConfig.SelectionLayerMask);

            return hitsCount != 0;
        }

        private bool TryIdentifySelection(int hitsCount, out int selected, out BattleSelectionType selectionType)
        {
            selected = -1;
            selectionType = BattleSelectionType.None;
            
            var hit = _raycastHits[0].transform;
            if (!hit.TryGetComponent<BattleSelectView>(out var selectView))
            {
                LogService.LogDebug(DebugType.Warning, $"Cannot get select component for {hit.gameObject.name}");
                return false;
            }

            if (!_ecsService.TryUnpack(selectView.Entity, out selected))
            {
                LogService.LogDebug(DebugType.Warning, $"Cannot unpack entity for {hit.gameObject.name}");
                return false;
            }

            selectionType = selectView.SelectionType;
            return true;
        }

        private void MarkSelected(int selected, BattleSelectionType selectionType)
        {
            if (_battleSelectedPool.Has(selected))
                return;
            
            ref var selectedComponent = ref _battleSelectedPool.Add(selected);
            ref var battleSelectEventComponent = ref _battleSelectEventPool.Add(selected);
            selectedComponent.SelectionType = selectionType;
            battleSelectEventComponent.SelectionType = selectionType;
        }
    }
}