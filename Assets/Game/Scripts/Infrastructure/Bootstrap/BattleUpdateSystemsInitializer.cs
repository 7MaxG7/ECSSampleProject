using Abstractions;
using Battle;
using Dices;
using Leopotam.EcsLite;
using UI.Battle;
using UI.Units;
using Zenject;

namespace Infrastructure
{
    public class BattleUpdateSystemsInitializer : IUpdateSystemsInitializer
    {
        private readonly DiContainer _container;

        [Inject]
        public BattleUpdateSystemsInitializer(DiContainer container)
        {
            _container = container;
        }

        public void InitSystems(EcsSystems updateSystems)
        {
            updateSystems
                .Add(_container.Resolve<BattleBeginStateSystem>())
                .Add(_container.Resolve<BattleDiceRollStateSystem>())
                .Add(_container.Resolve<BattleTargetSelectStateSystem>())
                .Add(_container.Resolve<DiceApplyStateSystem>())
                .Add(_container.Resolve<DiceAimingViewSystem>())
                .Add(_container.Resolve<DiceApplySystem>())
                .Add(_container.Resolve<DamageSystem>())
                .Add(_container.Resolve<DeathSystem>())
                
                // Views
                .Add(_container.Resolve<DiceApplyViewSystem>())
                .Add(_container.Resolve<UnitAnimationLaunchViewSystem>())

                // Post update
                .Add(_container.Resolve<BattleUIUpdater>())
                .Add(_container.Resolve<BattleDicesUIUpdater>())
                .Add(_container.Resolve<BattleUnitOverlayUIUpdater>())
                .Add(_container.Resolve<AnimationClearSystem>())

                .Add(_container.Resolve<DeleteFrameEventSystem>())
                .Init();
        }
    }
}