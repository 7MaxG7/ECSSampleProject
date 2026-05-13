using Battle;
using Battle.Battlefield;
using Dices;
using UI.Battle;
using Units;
using Units.Factories;
using Zenject;

namespace Infrastructure.Installers
{
    public class BattleInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<HighlightService>().AsSingle();
            Container.Bind<HighlightConfig>().FromScriptableObjectResource(nameof(HighlightConfig)).AsSingle();
            
            // Damage
            Container.Bind<HealthService>().AsSingle();
            Container.Bind<DamageService>().AsSingle();
            Container.Bind<BattleDeathService>().AsSingle();
            Container.BindInterfacesAndSelfTo<DamageSystem>().AsSingle();
            Container.BindInterfacesAndSelfTo<DeathSystem>().AsSingle();
            Container.BindInterfacesAndSelfTo<UnitAnimationLaunchViewSystem>().AsSingle();

            // Selection
            Container.Bind<BattleSelectionService>().AsSingle();
            Container.BindInterfacesAndSelfTo<BattleSelectionViewSystem>().AsSingle();
            
            // Dice
            Container.Bind<DiceApplyService>().AsSingle();
            Container.Bind<DiceApplyViewService>().AsSingle();
            Container.BindInterfacesAndSelfTo<DiceApplySystem>().AsSingle();
            Container.Bind<BattleDiceRollService>().AsSingle();
            Container.Bind<DiceRollsConfig>().FromScriptableObjectResource(nameof(DiceRollsConfig)).AsSingle();
            Container.Bind<BattleDiceLockService>().AsSingle();
            Container.Bind<BattleDiceService>().AsSingle();

            Container.Bind<DiceViewService>().AsSingle();
            Container.BindInterfacesAndSelfTo<DiceTargetSelectViewSystem>().AsSingle();
            Container.Bind<DiceTargetSelectService>().AsSingle();

            // Factories
            Container.Bind<BattlefieldViewFactory>().AsSingle();
            Container.Bind<BattlefieldFactory>().AsSingle();
            Container.Bind<UnitViewFactory>().AsSingle();
            Container.Bind<BattleUIFactory>().AsSingle();

            // Battlefield
            Container.Bind<BattlefieldBuilder>().AsSingle();
            Container.Bind<UnitSpawner>().AsSingle();
            Container.Bind<BattlefieldViewService>().AsSingle();
            Container.Bind<BattlefieldViewConfig>().FromScriptableObjectResource(nameof(BattlefieldViewConfig)).AsSingle();
            
            // Units
            Container.Bind<UnitViewService>().AsSingle();
            Container.Bind<BattleAnimatorService>().AsSingle();
            Container.Bind<BattleAnimationConfig>().FromScriptableObjectResource(nameof(BattleAnimationConfig)).AsSingle();
            
            Container.Bind<TeamBuilder>().AsSingle();
        }
    }
}