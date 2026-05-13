using UnityEngine;

namespace Battle
{
    public class HighlightView : MonoBehaviour
    {
        [SerializeField] private Renderer _renderer;
        
        public void EnableHighlight(Color color)
        {
            if (_renderer)
                _renderer.material.color = color;
            gameObject.SetActive(true);
        }

        public void DisableHighlight()
            => gameObject.SetActive(false);
    }
}