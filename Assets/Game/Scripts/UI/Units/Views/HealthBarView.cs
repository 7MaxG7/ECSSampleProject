using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Units
{
    public class HealthBarView : MonoBehaviour
    {
        [SerializeField] private Image _incomingArmoredDamageBar;
        [SerializeField] private Image _incomingDamageBar;
        [SerializeField] private Image _armorBar;
        [SerializeField] private Image _currentHpBar;

        private float _healthBarAnimationDuration;
        private bool _isAnimating;

        public void Init(float healthBarAnimationDuration)
        {
            _healthBarAnimationDuration = healthBarAnimationDuration;
            _incomingArmoredDamageBar.fillAmount = 0f;
            _incomingDamageBar.fillAmount = 0f;
            _armorBar.fillAmount = 0f;
        }

        public void Clear()
            => KillDoTween();

        public async UniTask UpdateBarsAsync(int barsCapacity, int armoredDamage, int damage, int armor, int currentHp,
            CancellationTokenSource cts)
        {
            KillDoTween();

            var capacity = (float)barsCapacity;
            var newCurrentHp = currentHp / capacity;
            var newArmor = armor / capacity;
            var newDamage = damage / capacity;
            var newArmoredDamage = armoredDamage / capacity;

            if (newCurrentHp < _currentHpBar.fillAmount || newArmor < _armorBar.fillAmount)
            {
                await UpdateArmoredDamageAsync(newArmoredDamage, cts);
                await UpdateDamageAsync(newDamage, cts);
                await UpdateArmorAsync(newArmor, cts);
                await UpdateCurrentHpAsync(newCurrentHp, cts);
            }
            else
            {
                await UpdateCurrentHpAsync(newCurrentHp, cts);
                await UpdateArmorAsync(newArmor, cts);
                await UpdateArmoredDamageAsync(newArmoredDamage, cts);
                await UpdateDamageAsync(newDamage, cts);
            }
        }

        private async UniTask UpdateArmoredDamageAsync(float newArmoredDamage, CancellationTokenSource cts)
        {
            if (newArmoredDamage < _incomingArmoredDamageBar.fillAmount)
                await _incomingArmoredDamageBar.DOFillAmount(newArmoredDamage, _healthBarAnimationDuration).WithCancellation(cts.Token);
            else
                _incomingArmoredDamageBar.fillAmount = newArmoredDamage;
        }

        private async UniTask UpdateDamageAsync(float newDamage, CancellationTokenSource cts)
        {
            if (newDamage < _incomingDamageBar.fillAmount)
                await _incomingDamageBar.DOFillAmount(newDamage, _healthBarAnimationDuration).WithCancellation(cts.Token);
            else
                _incomingDamageBar.fillAmount = newDamage;
        }

        private async UniTask UpdateCurrentHpAsync(float newCurrentHp, CancellationTokenSource cts)
            => await _currentHpBar.DOFillAmount(newCurrentHp, _healthBarAnimationDuration).WithCancellation(cts.Token);

        private async UniTask UpdateArmorAsync(float newArmor, CancellationTokenSource cts)
            => await _armorBar.DOFillAmount(newArmor, _healthBarAnimationDuration).WithCancellation(cts.Token);

        private void KillDoTween()
        {
            _incomingDamageBar.DOKill();
            _armorBar.DOKill();
            _currentHpBar.DOKill();
        }
    }
}