using System.Threading;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace UI.Battle
{
    public class BattleEndUIView : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private TMP_Text _label;

        private float _fadeDuration;

        public void Init(float fadeDuration)
        {
            _fadeDuration = fadeDuration;
        }

        public async UniTaskVoid SetActiveAsync(bool isActive, CancellationToken token)
            => await UiAnimationUtility.ToggleCanvasGroupVisibilityAsync(_canvasGroup, isActive, _fadeDuration, token);

        public void SetWinnerLabel(string text)
            => _label.text = text;

        public void SetVisible(bool isVisible)
            => gameObject.SetActive(isVisible);
    }
}