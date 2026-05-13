using CustomTypes;
using Infrastructure;
using Leopotam.EcsLite;
using UnityEngine;

namespace Battle
{
    public class HighlightService
    {
        private readonly HighlightConfig _highlightConfig;
        
        private readonly EcsPool<HighlightViewComponent> _viewHighlightPool;

        public HighlightService(EcsService ecsService, HighlightConfig highlightConfig)
        {
            _highlightConfig = highlightConfig;
            
            _viewHighlightPool = ecsService.World.GetPool<HighlightViewComponent>();
        }

        public void InitComponents(int entity, HighlightView highlightView)
        {
            ref var viewHighlightComponent = ref _viewHighlightPool.Add(entity);
            viewHighlightComponent.Highlight = highlightView;
            highlightView.DisableHighlight();
        }

        public void Clear(int entity)
            => _viewHighlightPool.Del(entity);

        public void SetHighlight(int entity, bool mustEnabled, HighlightType highlightType = HighlightType.Default)
        {
            if (!mustEnabled)
            {
                
            }
            
            ref var viewHighlightComponent = ref _viewHighlightPool.Get(entity);

            var color = highlightType switch
            {
                HighlightType.Default => _highlightConfig.DefaultHighlightColor,
                HighlightType.Aiming => _highlightConfig.AimingHighlightColor,
                _ => Color.clear,
            };
            
            if (mustEnabled)
                viewHighlightComponent.Highlight.EnableHighlight(color);
            else
                viewHighlightComponent.Highlight.DisableHighlight();
        }
    }
}