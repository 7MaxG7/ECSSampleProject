using UI;
using UI.Battle;
using UI.Units;
using Zenject;

namespace Infrastructure
{
    public class BattleUIInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<BattleUIModel>().AsSingle();
            Container.Bind<BattleUIBuilder>().AsSingle();
            Container.Bind<BattleUIController>().AsSingle();
            Container.Bind<BattleEndUIController>().AsSingle();
            Container.Bind<UnitsOverlayUIController>().AsSingle();
            Container.Bind<BattleDicesUIController>().AsSingle();
            Container.BindInterfacesAndSelfTo<BattleUIUpdater>().AsSingle();
            Container.BindInterfacesAndSelfTo<BattleDicesUIUpdater>().AsSingle();
            Container.BindInterfacesAndSelfTo<BattleUnitOverlayUIUpdater>().AsSingle();
            Container.Bind<UnitOverlayUIService>().AsSingle();
            Container.Bind<BattleUIAssetsDb>().FromScriptableObjectResource(nameof(BattleUIAssetsDb)).AsSingle();
        }
    }
}