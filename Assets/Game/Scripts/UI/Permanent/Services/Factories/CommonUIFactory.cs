using Infrastructure;
using UnityEngine;
using Zenject;

namespace UI.Permanent
{
    public class CommonUIFactory
    {
        private readonly UIAssetsDb _uiAssetsDb;
        private readonly UIConfig _uiConfig;
        private readonly Instantiator _instantiator;

        [Inject]
        public CommonUIFactory(UIAssetsDb uiAssetsDb, UIConfig uiConfig, Instantiator instantiator)
        {
            _uiAssetsDb = uiAssetsDb;
            _uiConfig = uiConfig;
            _instantiator = instantiator;
        }

        public PermanentUIView CreateRoot()
        {
            var permanentUI = _instantiator.Create(_uiAssetsDb.PermanentUIView);

            Object.DontDestroyOnLoad(permanentUI.gameObject);
            return permanentUI;
        }

        public CurtainUIView CreateCurtain(Transform parent)
        {
            var curtainView = _instantiator.Create(_uiAssetsDb.CurtainView, parent);
            curtainView.Init(_uiConfig.DefaultAnimationDuration);
            return curtainView;
        }
    }
}