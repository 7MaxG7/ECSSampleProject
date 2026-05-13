using Infrastructure;
using UnityEngine;

namespace UI.Utils.Extensions
{
    public static class UIExtensions
    {
        public static bool IsVisible(this CanvasGroup canvasGroup) 
            => canvasGroup.gameObject.activeSelf && canvasGroup.alpha > Constants.ALMOST_ONE;
    }
}