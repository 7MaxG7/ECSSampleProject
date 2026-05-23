using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Units
{
    public class HealthBarView : MonoBehaviour
    {
        [SerializeField] private Image _incomingDamageBar;
        [SerializeField] private Image _armorBar;
        [SerializeField] private Image _currentHpBar;

        private float _healthBarAnimationDuration;
        private bool _isAnimating;

        public void Init(float healthBarAnimationDuration)
        {
            _healthBarAnimationDuration = healthBarAnimationDuration;
        }

        public void Clear()
            => KillDoTween();

        public async UniTask UpdateBarsAsync(int barsCapacity, int damage, int armor, int currentHp,
            CancellationTokenSource cts)
        {
            KillDoTween();

            var capacity = (float)barsCapacity;
            var newDamage = damage / capacity;

            if (newDamage < _incomingDamageBar.fillAmount)
                await _incomingDamageBar.DOFillAmount(newDamage, _healthBarAnimationDuration).WithCancellation(cts.Token);
            else
                _incomingDamageBar.fillAmount = newDamage;

            await _armorBar.DOFillAmount(armor / capacity, _healthBarAnimationDuration).WithCancellation(cts.Token);
            await _currentHpBar.DOFillAmount(currentHp / capacity, _healthBarAnimationDuration).WithCancellation(cts.Token);
        }

        private void KillDoTween()
        {
            _incomingDamageBar.DOKill();
            _armorBar.DOKill();
            _currentHpBar.DOKill();
        }
    }
}