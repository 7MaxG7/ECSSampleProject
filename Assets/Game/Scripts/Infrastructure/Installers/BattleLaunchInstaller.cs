using Zenject;

namespace Infrastructure
{
    public class BattleLaunchInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesTo<BattleRunner>().AsSingle();
        }
    }
}