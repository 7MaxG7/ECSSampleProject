using System.Collections.Generic;
using System.Linq;
using System.Threading;
using CustomTypes;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using Infrastructure;
using UI.Battle;
using UnityEngine;

namespace UI.Units
{
    public class UnitOverlayUIController
    {
        private readonly UIConfig _uiConfig;
        private readonly CancellationTokenProvider _tokenProvider;
        private readonly BattleUIFactory _battleUIFactory;

        private UnitUIOverlayModel _model;
        private UnitOverlayUIView _view;
        private HealthBarUIController _healthBarUIController;
        private readonly List<OverlayDiceFacetUIView> _facetViews = new();
        private readonly Dictionary<DiceSideType, Sprite> _facetIcons;

        public UnitOverlayUIController(UIConfig uiConfig, CancellationTokenProvider tokenProvider, BattleUIFactory battleUIFactory)
        {
            _uiConfig = uiConfig;
            _tokenProvider = tokenProvider;
            _battleUIFactory = battleUIFactory;

            _facetIcons = uiConfig.DiceSideIcons.ToDictionary(data => data.DiceSide, data => data.Icon);
        }

        public void Init(UnitUIOverlayModel model, UnitOverlayUIView view)
        {
            _model = model;
            _view = view;

            var cts = _tokenProvider.CreateLocalCts();
            model.AreFacetModelsAdded.Subscribe(AddFacets, cts.Token);
            model.IsOverlayVisible.Subscribe(view.SetVisible, cts.Token);

            _healthBarUIController = new HealthBarUIController(_uiConfig, _tokenProvider);
            _healthBarUIController.Init(model, view.HealthBar);
        }

        public void Clear()
        {
            _healthBarUIController.Clear();
        }

        private async UniTaskVoid AddFacets(CancellationToken token)
        {
            for (var i = _facetViews.Count; i < _model.FacetModels.Count; i++)
            {
                var view = await _battleUIFactory.CreateOverlayDiceFacetAsync(_view.FacetIconsContent);
                view.transform.SetSiblingIndex(0);  // Latest targeted dices must be on the right side (it's models are earlier)
                var model = _model.FacetModels[i];
                
                model.IsVisible.Subscribe(view.SetVisible, token);
                model.DiceSideType.Subscribe(sideType =>
                {
                    var icon = _facetIcons.GetValueOrDefault(sideType);
                    if (icon == null)
                        LogService.LogDebug(DebugType.Error, $"Facet icon {sideType} not found");
                    view.SetIcon(icon);
                }, token);
                
                _facetViews.Add(view);
            }
        }
    }
}