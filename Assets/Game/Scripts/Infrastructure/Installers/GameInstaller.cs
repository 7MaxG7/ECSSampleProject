using Battle;
using Battle.Battlefield;
using Dices;
using UI.Permanent;
using Units;
using Zenject;

namespace Infrastructure
{
    public class GameInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            // Common
            Container.Bind<TeamService>().AsSingle();

            // Input
            Container.Bind<InputService>().AsSingle();
            Container.Bind<SelectionConfig>().FromScriptableObjectResource(nameof(SelectionConfig)).AsSingle();
            
            // Factories
            Container.Bind<UnitFactory>().AsSingle();
            Container.Bind<DiceFactory>().AsSingle();
            Container.Bind<CommonUIFactory>().AsSingle();

            // Battlefield
            Container.Bind<BattleCellService>().AsSingle();
            Container.Bind<BattleLocationService>().AsSingle();
            Container.Bind<BattlefieldConfig>().FromScriptableObjectResource(nameof(BattlefieldConfig)).AsSingle();
            
            // Units
            Container.Bind<UnitService>().AsSingle();
        }
    }
}