using System;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using Infrastructure;
using Utils.Extensions;

namespace UI.Units
{
    public class HealthBarUIController
    {
        private readonly HealthBarUIModel _healthBarUIModel;
        private readonly HealthBarView _healthBarView;
        private readonly UIConfig _uiConfig;
        private readonly CancellationTokenProvider _tokenProvider;

        public HealthBarUIController(HealthBarUIModel healthBarUIModel, HealthBarView healthBarView, UIConfig uiConfig,
            CancellationTokenProvider tokenProvider)
        {
            _healthBarUIModel = healthBarUIModel;
            _healthBarView = healthBarView;
            _uiConfig = uiConfig;
            _tokenProvider = tokenProvider;
        }

        public void Init()
        {
            _healthBarView.Init(_uiConfig.HealthBarAnimationDuration);

            _healthBarUIModel.CurrentHp.Subscribe(SetCurrentHp);
            _healthBarUIModel.MaxHp.Subscribe(SetMaxHp);
            _healthBarUIModel.Armor.Subscribe(SetArmor);
            _healthBarUIModel.IncomingDamage.Subscribe(SetIncomingDamage);
        }

        public void Clear()
        {
            _healthBarView.Clear();
        }

        public void UpdateHealth(int currentHp, int maxHp, int armor)
        {
            _healthBarUIModel.CurrentHp.Update(currentHp);
            if (currentHp <= 0)
                return;

            _healthBarUIModel.MaxHp.Update(maxHp);
            _healthBarUIModel.Armor.Update(armor);
        }

        public void UpdateDamage(int damage)
            => _healthBarUIModel.IncomingDamage.Update(damage);

        private void SetCurrentHp(int hp)
        {
            if (hp <= 0)
                _healthBarView.Disable();
            else
                CalculateBarsAsync(hp, _healthBarUIModel.Armor, _healthBarUIModel.IncomingDamage, _healthBarUIModel.MaxHp).Forget();
        }

        private void SetMaxHp(int maxHp)
            => CalculateBarsAsync(_healthBarUIModel.CurrentHp, _healthBarUIModel.Armor, _healthBarUIModel.IncomingDamage, maxHp).Forget();

        private void SetArmor(int armor)
            => CalculateBarsAsync(_healthBarUIModel.CurrentHp, armor, _healthBarUIModel.IncomingDamage, _healthBarUIModel.MaxHp).Forget();

        private void SetIncomingDamage(int damage)
            => CalculateBarsAsync(_healthBarUIModel.CurrentHp, _healthBarUIModel.Armor, damage, _healthBarUIModel.MaxHp).Forget();

        private async UniTaskVoid CalculateBarsAsync(int currentHp, int armor, int damage, int maxHp)
        {
            if (currentHp <= 0)
                return;

            using var cts = _tokenProvider.CreateLocalCts();
            var barsCapacity = Math.Max(maxHp, currentHp + armor);
            var damageBar = damage == 0 ? 0 : currentHp + armor;
            var armorBar = currentHp + armor - damage;
            var currentHpBar = Math.Min(currentHp - damage + armor, currentHp);

            await _healthBarView.UpdateBarsAsync(barsCapacity, damageBar, armorBar, currentHpBar, cts);
        }
    }
}