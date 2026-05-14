using Battle;
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
            
            Container.BindInterfacesAndSelfTo<DeleteFrameEventSystem>().AsSingle();
        }
    }
}