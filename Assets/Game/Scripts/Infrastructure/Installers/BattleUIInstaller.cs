using Dices;
using UI;
using UI.Battle;
using UI.Units;
using Zenject;

namespace Infrastructure.Installers
{
    public class BattleUIInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<BattleUIBuilder>().AsSingle();
            Container.Bind<BattleUIController>().AsSingle();
            Container.Bind<BattleEndUIController>().AsSingle();
            Container.Bind<UnitsOverlayUIController>().AsSingle();
            Container.Bind<BattleDicesUIController>().AsSingle();
            Container.Bind<DiceAimingService>().AsSingle();
            Container.BindInterfacesAndSelfTo<BattleUIUpdateSystem>().AsSingle();
            Container.BindInterfacesAndSelfTo<UnitsHealthBarUpdateSystem>().AsSingle();
            Container.Bind<UnitOverlayUIService>().AsSingle();
            Container.Bind<BattleUIAssetsDb>().FromScriptableObjectResource(nameof(BattleUIAssetsDb)).AsSingle();
        }
    }
}