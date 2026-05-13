using Battle;
using Dices;
using Units;
using Zenject;

namespace Infrastructure
{
    public class BattleInfrastructureInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            // Initializers
            Container.Bind<BattleUpdateSystemsInitializer>().AsSingle();
            Container.Bind<BattleFixedUpdateSystemsInitializer>().AsSingle();
  
            // States
            Container.Bind<BattleStateMachine>().AsSingle();
            Container.Bind<BattleDiceRollState>().AsSingle();
            Container.Bind<TargetSelectState>().AsSingle();
            Container.Bind<DicesApplyState>().AsSingle();
            Container.Bind<EndBattleState>().AsSingle();
            Container.BindInterfacesAndSelfTo<BattleDiceRollStateSystem>().AsSingle();
            Container.BindInterfacesAndSelfTo<BattleTargetSelectStateSystem>().AsSingle();
            Container.BindInterfacesAndSelfTo<DiceApplyStateSystem>().AsSingle();

            // Frame event systems
            Container.BindInterfacesAndSelfTo<DeleteFrameEventSystem<DicesRollEventComponent>>().AsSingle();
            Container.BindInterfacesAndSelfTo<DeleteFrameEventSystem<DiceLockEventComponent>>().AsSingle();
            Container.BindInterfacesAndSelfTo<DeleteFrameEventSystem<BattleSelectEventComponent>>().AsSingle();
            Container.BindInterfacesAndSelfTo<DeleteFrameEventSystem<BattleDeselectEventComponent>>().AsSingle();
            Container.BindInterfacesAndSelfTo<DeleteFrameEventSystem<DiceAimingEventComponent>>().AsSingle();
            Container.BindInterfacesAndSelfTo<DeleteFrameEventSystem<DiceUnaimingEventComponent>>().AsSingle();
            Container.BindInterfacesAndSelfTo<DeleteFrameEventSystem<DiceTargetedEventComponent>>().AsSingle();
            Container.BindInterfacesAndSelfTo<DeleteFrameEventSystem<DeathEventComponent>>().AsSingle();
            Container.BindInterfacesAndSelfTo<DeleteFrameEventSystem<AnimationLaunchEventComponent>>().AsSingle();
            Container.BindInterfacesAndSelfTo<DeleteFrameEventSystem<AnimationActionReadyEventComponent>>().AsSingle();
        }
    }
}