using System.Collections.Generic;
using System.Threading;
using Abstractions;
using CustomTypes;
using Cysharp.Threading.Tasks;
using Infrastructure;
using UI.Units;
using Units;
using UnityEngine;
using Zenject;

namespace UI.Battle
{
    public class UnitsOverlayUIController
    {
        private readonly UnitOverlayUIService _unitOverlayUIService;
        private readonly BattleUIFactory _battleUIFactory;
        private readonly UIConfig _uiConfig;
        private readonly UnitService _unitService;
        private readonly CancellationTokenProvider _tokenProvider;

        private IUnitsOverlayUIModel _model;
        private BattleUnitsOverlayUIView _view;
        private readonly Dictionary<string, UnitOverlayUIController> _overlayControllers = new();

        [Inject]
        public UnitsOverlayUIController(UnitOverlayUIService unitOverlayUIService, UnitService unitService, BattleUIFactory battleUIFactory,
            CancellationTokenProvider tokenProvider, UIConfig uiConfig)
        {
            _unitOverlayUIService = unitOverlayUIService;
            _battleUIFactory = battleUIFactory;
            _uiConfig = uiConfig;
            _unitService = unitService;
            _tokenProvider = tokenProvider;
        }

        public void Init(IUnitsOverlayUIModel model, BattleUnitsOverlayUIView view)
        {
            _model = model;
            _view = view;

            var cts = _tokenProvider.CreateLocalCts();
            model.AreUnitOverlayModelsAdded.Subscribe(AddUnitOverlaysAsync, cts.Token);
        }

        public void Clear()
        {
            foreach (var controller in _overlayControllers.Values)
                controller.Clear();
        }

        private async UniTaskVoid AddUnitOverlaysAsync(CancellationToken token)
        {
            foreach (var (unitId, model) in _model.UnitOverlayModels)
            {
                if (_overlayControllers.ContainsKey(unitId))
                    continue;

                if (!_unitService.TryGetUnit(unitId, out var unit))
                {
                    LogService.LogDebug(DebugType.Error, $"Unit {unitId} not found to create overlay");
                    continue;
                }

                var controller = new UnitOverlayUIController(_uiConfig, _tokenProvider, _battleUIFactory);
                _overlayControllers.Add(unitId, controller);
                
                var uiOverlayAnchor = _unitOverlayUIService.GetOverlayAnchor(unit);
                var position = Camera.main!.WorldToScreenPoint(uiOverlayAnchor.position);
                var view = await _battleUIFactory.CreateUnitOverlayViewAsync(position, _view.OverlaysContent);
                controller.Init(model, view);

            }
        }
    }
}