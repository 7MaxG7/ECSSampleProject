using System;
using Abstractions;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using Infrastructure;

namespace UI.Units
{
    public class HealthBarUIController
    {
        private readonly UIConfig _uiConfig;
        private readonly CancellationTokenProvider _tokenProvider;
        
        private IHealthBarUIModel _model;
        private HealthBarView _view;

        public HealthBarUIController(UIConfig uiConfig, CancellationTokenProvider tokenProvider)
        {
            _uiConfig = uiConfig;
            _tokenProvider = tokenProvider;
        }

        public void Init(IHealthBarUIModel model, HealthBarView view)
        {
            _model = model;
            _view = view;
            _view.Init(_uiConfig.HealthBarAnimationDuration);

            _model.CurrentHp.Subscribe(SetCurrentHp);
            _model.MaxHp.Subscribe(SetMaxHp);
            _model.Armor.Subscribe(SetArmor);
            _model.IncomingDamage.Subscribe(SetIncomingDamage);
        }

        public void Clear()
        {
            _view.Clear();
        }

        private void SetCurrentHp(int hp)
            => CalculateBarsAsync(hp, _model.Armor, _model.IncomingDamage, _model.MaxHp).Forget();

        private void SetMaxHp(int maxHp)
            => CalculateBarsAsync(_model.CurrentHp, _model.Armor, _model.IncomingDamage, maxHp).Forget();

        private void SetArmor(int armor)
            => CalculateBarsAsync(_model.CurrentHp, armor, _model.IncomingDamage, _model.MaxHp).Forget();

        private void SetIncomingDamage(int damage)
            => CalculateBarsAsync(_model.CurrentHp, _model.Armor, damage, _model.MaxHp).Forget();

        private async UniTaskVoid CalculateBarsAsync(int currentHp, int armor, int damage, int maxHp)
        {
            currentHp = Math.Max(currentHp, 0);

            var barsCapacity = Math.Max(maxHp, currentHp + armor);
            var damageBar = damage == 0 ? 0 : currentHp + armor;
            var armorBar = currentHp + armor - damage;
            var currentHpBar = Math.Min(currentHp - damage + armor, currentHp);

            await _view.UpdateBarsAsync(barsCapacity, damageBar, armorBar, currentHpBar, _tokenProvider.CreateLocalCts());
        }
    }
}