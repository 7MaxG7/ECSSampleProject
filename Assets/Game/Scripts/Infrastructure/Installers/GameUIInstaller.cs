using UI;
using UI.Permanent;
using Zenject;

namespace Infrastructure
{
    public class GameUIInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<UiAnimationUtility>().AsSingle();
            Container.Bind<UIAssetsDb>().FromScriptableObjectResource(nameof(UIAssetsDb)).AsSingle();
            Container.Bind<UIConfig>().FromScriptableObjectResource(nameof(UIConfig)).AsSingle();
            Container.Bind<CurtainService>().AsSingle();
            Container.Bind<PermanentUIBuilder>().AsSingle();
        }
    }
}