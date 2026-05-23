using System.Collections.Generic;
using CustomTypes;
using Infrastructure;
using Leopotam.EcsLite;
using UnityEngine;

namespace Battle
{
    public class HighlightService
    {
        private readonly EcsPool<HighlightViewComponent> _viewHighlightPool;
        private readonly Dictionary<HighlightType, Color> _highlightColors;

        public HighlightService(EcsService ecsService, HighlightConfig highlightConfig)
        {
            _viewHighlightPool = ecsService.World.GetPool<HighlightViewComponent>();
            
            _highlightColors = new Dictionary<HighlightType, Color>()
            {
                [HighlightType.Default] = highlightConfig.DefaultHighlightColor,
                [HighlightType.Aiming] = highlightConfig.AimingHighlightColor,
            };
        }

        public void InitComponents(int entity, HighlightView highlightView)
        {
            ref var viewHighlightComponent = ref _viewHighlightPool.Add(entity);
            viewHighlightComponent.Highlight = highlightView;
            highlightView.Init(_highlightColors);
            highlightView.DisableHighlight();
        }

        public void Clear(int entity)
            => _viewHighlightPool.Del(entity);

        public void SetHighlight(int entity, bool mustEnabled, HighlightType highlightType = HighlightType.Default)
        {
            ref var viewHighlightComponent = ref _viewHighlightPool.Get(entity);
            
            if (mustEnabled)
                viewHighlightComponent.Highlight.EnableHighlight(highlightType);
            else
                viewHighlightComponent.Highlight.DisableHighlight();
        }
    }
}