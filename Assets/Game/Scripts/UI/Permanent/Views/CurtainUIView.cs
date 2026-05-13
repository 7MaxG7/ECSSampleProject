using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace UI.Permanent
{
    public class CurtainUIView : MonoBehaviour
    {
		[SerializeField] private CanvasGroup _canvasGroup;

		private float _fadeDuration;

		public void Init(float fadeDuration)
		{
			_fadeDuration = fadeDuration;

			_canvasGroup.alpha = 0;
			gameObject.SetActive(false);
		}

		public void Clear()
		{
			_canvasGroup.DOKill();
		}

		public async UniTask ToggleActiveAsync(bool isActive, CancellationTokenSource cts)
			=> await UiAnimationUtility.ToggleCanvasGroupVisibilityAsync(_canvasGroup, isActive, _fadeDuration, cts);

		public void ShowInstantly()
		{
			_canvasGroup.alpha = 1f;
			gameObject.SetActive(true);
		}
    }
}