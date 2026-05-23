using UnityEngine;
using UnityEngine.UI;

namespace UI.Units
{
    public class OverlayDiceFacetUIView : MonoBehaviour
    {
        [SerializeField] private Image _icon;

        public void SetVisible(bool isVisible)
            => gameObject.SetActive(isVisible);

        public void SetIcon(Sprite icon)
            => _icon.sprite = icon;
    }
}