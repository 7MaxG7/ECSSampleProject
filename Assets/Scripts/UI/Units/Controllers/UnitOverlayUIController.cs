using Infrastructure;

namespace UI.Units
{
    public class UnitOverlayUIController
    {
        private readonly UnitOverlayUIView _unitOverlayUIView;
        private readonly UIConfig _uiConfig;
        private readonly CancellationTokenProvider _tokenProvider;

        private HealthBarUIController _healthBarUIController;

        public UnitOverlayUIController(UnitOverlayUIView unitOverlayUIView, UIConfig uiConfig, CancellationTokenProvider tokenProvider)
        {
            _unitOverlayUIView = unitOverlayUIView;
            _uiConfig = uiConfig;
            _tokenProvider = tokenProvider;
        }

        public void Init()
        {
            _healthBarUIController =
                new HealthBarUIController(new HealthBarUIModel(), _unitOverlayUIView.HealthBar, _uiConfig, _tokenProvider);
            _healthBarUIController.Init();
        }

        public void Clear()
        {
            _healthBarUIController.Clear();
        }

        public void UpdateHealth(int currentHp, int maxHp, int armor)
            => _healthBarUIController.UpdateHealth(currentHp, maxHp, armor);

        public void UpdateDamage(int damage)
            => _healthBarUIController.UpdateDamage(damage);
    }
}