using System.Collections.Generic;
using CustomTypes;
using UnityEngine;

namespace Battle
{
    public class HighlightView : MonoBehaviour
    {
        [SerializeField] private Renderer _renderer;
        
        private Dictionary<HighlightType, Color> _highlightColors;

        public void Init(Dictionary<HighlightType, Color> highlightColors)
        {
            _highlightColors = highlightColors;
        }

        public void EnableHighlight(HighlightType highlightType)
        {
            if (_renderer)
            {
                if (!_highlightColors.TryGetValue(highlightType, out var color))
                    color = Color.white;

                _renderer.material.color = color;
            }
            gameObject.SetActive(true);
        }

        public void DisableHighlight()
            => gameObject.SetActive(false);

        public void SetHighlightEnabled(bool isEnabled)
            => gameObject.SetActive(isEnabled);
    }
}