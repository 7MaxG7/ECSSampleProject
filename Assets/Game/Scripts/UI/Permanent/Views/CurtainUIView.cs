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

		public async UniTask SetActiveAsync(bool isActive, CancellationToken token)
			=> await UiAnimationUtility.ToggleCanvasGroupVisibilityAsync(_canvasGroup, isActive, _fadeDuration, token);

		public void SetActiveInstantly(bool isActive)
		{
			_canvasGroup.alpha = isActive ? 1f : 0f;
			gameObject.SetActive(isActive);
		}
    }
}