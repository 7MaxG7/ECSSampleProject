using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UI.Utils;
using UnityEngine;

namespace UI
{
    public class UiAnimationUtility
    {
        public void Init()
            => DOTween.Init();

        public void OnDispose()
            => DOTween.Clear();
        
        public static async UniTask ToggleCanvasGroupVisibilityAsync(CanvasGroup canvasGroup, bool mustVisible, float animationDuration,
            CancellationToken token)
        {
            canvasGroup.DOKill();
            if (mustVisible && !canvasGroup.IsVisible())
            {
                if (!canvasGroup.gameObject.activeSelf)
                {
                    canvasGroup.gameObject.SetActive(true);
                    canvasGroup.alpha = 0;
                }

                await canvasGroup.DOFade(1, animationDuration)
                    .SetUpdate(true)
                    .WithCancellation(token)
                    .SuppressCancellationThrow();
            }
            else if (!mustVisible && canvasGroup.gameObject.activeSelf)
            {
                await canvasGroup.DOFade(0, animationDuration)
                    .SetUpdate(true)
                    .WithCancellation(token)
                    .SuppressCancellationThrow();
                canvasGroup.gameObject.SetActive(false);
            }
        }
    }
}