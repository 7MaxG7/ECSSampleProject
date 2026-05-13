using Infrastructure.Bootstrap;
using Zenject;

namespace Infrastructure.Installers
{
    public class GameLaunchInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<GameBootstrapper>().AsSingle();
        }
    }
}