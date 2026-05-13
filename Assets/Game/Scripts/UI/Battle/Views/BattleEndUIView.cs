using System.Threading;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace UI.Battle
{
    public class BattleEndUIView : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private TMP_Text _lable;

        private float _fadeDuration;

        public void Init(float fadeDuration)
        {
            _fadeDuration = fadeDuration;
        }

        public async UniTask ShowAsync(CancellationTokenSource cts)
            => await UiAnimationUtility.ToggleCanvasGroupVisibilityAsync(_canvasGroup, true, _fadeDuration, cts);

        public void SetWinnerLabel(string text)
            => _lable.text = text;
    }
}