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
        private readonly FrameComponentsService _frameComponentsService;

        private readonly EcsFilter _selectedFilter;
        private readonly EcsPool<BattleSelectedComponent> _battleSelectedPool;

        private readonly RaycastHit[] _raycastHits;

        [Inject]
        public BattleSelectionService(EcsService ecsService, SelectionConfig selectionConfig, FrameComponentsService frameComponentsService,
            InputService inputService)
        {
            _ecsService = ecsService;
            _selectionConfig = selectionConfig;
            _inputService = inputService;
            _frameComponentsService = frameComponentsService;

            _selectedFilter = ecsService.World.Filter<BattleSelectedComponent>().End();
            _battleSelectedPool = ecsService.World.GetPool<BattleSelectedComponent>();

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
                ref var battleSelectedComponent = ref _battleSelectedPool.Get(selected);
                ref var battleDeselectEventComponent = ref _frameComponentsService.AddEvent<BattleDeselectEventComponent>(selected);
                battleDeselectEventComponent.SelectionType = battleSelectedComponent.SelectionType;
                _battleSelectedPool.Del(selected);
            }
        }

        private bool TryRaycastSelection(out int hitsCount)
        {
            // ReSharper disable once PossibleNullReferenceException
            var ray = Camera.main.ScreenPointToRay(_inputService.MousePosition);
            hitsCount = Physics.RaycastNonAlloc(ray, _raycastHits, _selectionConfig.SelectionRaycastLength,
                _selectionConfig.SelectionLayerMask);

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
            selectedComponent.SelectionType = selectionType;
            _frameComponentsService.AddAddedEvent<BattleSelectedComponent>(selected);
        }
    }
}