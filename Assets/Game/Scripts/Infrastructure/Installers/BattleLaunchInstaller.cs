using Infrastructure.Bootstrap;
using Zenject;

namespace Infrastructure.Installers
{
    public class BattleLaunchInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesTo<BattleRunner>().AsSingle();
        }
    }
}