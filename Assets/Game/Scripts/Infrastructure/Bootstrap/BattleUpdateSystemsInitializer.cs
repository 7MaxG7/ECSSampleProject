using Abstractions;
using Battle;
using Dices;
using Leopotam.EcsLite;
using UI;
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
                .Add(_container.Resolve<BattleDiceRollStateSystem>())
                .Add(_container.Resolve<BattleTargetSelectStateSystem>())
                .Add(_container.Resolve<DiceApplyStateSystem>())
                .Add(_container.Resolve<BattleSelectionViewSystem>())
                .Add(_container.Resolve<DiceTargetSelectViewSystem>())
                .Add(_container.Resolve<DiceApplySystem>())
                .Add(_container.Resolve<DamageSystem>())
                .Add(_container.Resolve<DeathSystem>())
                .Add(_container.Resolve<UnitAnimationLaunchViewSystem>())

                // Post update
                .Add(_container.Resolve<BattleUIUpdateSystem>())
                .Add(_container.Resolve<UnitsHealthBarUpdateSystem>())

                .Add(_container.Resolve<DeleteFrameEventSystem>())
                .Init();
        }
    }
}