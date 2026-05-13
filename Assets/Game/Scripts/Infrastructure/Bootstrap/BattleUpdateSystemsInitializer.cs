using Abstractions.Infrastructure;
using Battle;
using Dices;
using Dices.Events;
using Leopotam.EcsLite;
using UI;
using UI.Units;
using Units;
using Zenject;

namespace Infrastructure.Bootstrap
{
    public class BattleUpdateSystemsInitializer : IUpdateSystemsInitializer
    {
        private readonly BattleDiceRollStateSystem _battleDiceRollStateSystem;
        private readonly DeathSystem _deathSystem;
        private readonly BattleTargetSelectStateSystem _battleTargetSelectStateSystem;
        private readonly DiceApplyStateSystem _diceApplyStateSystem;
        private readonly BattleUIUpdateSystem _battleUIUpdateSystem;
        private readonly BattleSelectionViewSystem _selectionViewSystem;
        private readonly DiceApplySystem _diceApplySystem;
        private readonly DiceTargetSelectViewSystem _diceTargetSelectViewSystem;
        private readonly DamageSystem _damageSystem;
        private readonly UnitAnimationLaunchViewSystem _unitAnimationLaunchViewSystem;
        private readonly UnitsHealthBarUpdateSystem _healthBarUpdateSystem;
        private readonly DeleteFrameEventSystem<DicesRollEventComponent> _deleteRollEventSystem;
        private readonly DeleteFrameEventSystem<DiceLockEventComponent> _diceLockEventSystem;
        private readonly DeleteFrameEventSystem<BattleSelectEventComponent> _battleSelectEventSystem;
        private readonly DeleteFrameEventSystem<BattleDeselectEventComponent> _battleDeselectEventSystem;
        private readonly DeleteFrameEventSystem<DiceAimingEventComponent> _diceAimingEventSystem;
        private readonly DeleteFrameEventSystem<DiceUnaimingEventComponent> _diceUnaimingEventSystem;
        private readonly DeleteFrameEventSystem<DiceTargetedEventComponent> _diceTargetedEventSystem;
        private readonly DeleteFrameEventSystem<AnimationLaunchEventComponent> _animationLaunchEventSystem;
        private readonly DeleteFrameEventSystem<AnimationActionReadyEventComponent> _animationActionReadyEventSystem;
        private readonly DeleteFrameEventSystem<DeathEventComponent> _deathEventSystem;

        [Inject]
        public BattleUpdateSystemsInitializer(BattleDiceRollStateSystem battleDiceRollStateSystem, DeathSystem deathSystem,
            BattleTargetSelectStateSystem battleTargetSelectStateSystem, DiceApplyStateSystem diceApplyStateSystem,
            BattleUIUpdateSystem battleUIUpdateSystem, BattleSelectionViewSystem selectionViewSystem, DiceApplySystem diceApplySystem,
            DiceTargetSelectViewSystem diceTargetSelectViewSystem, DamageSystem damageSystem, UnitAnimationLaunchViewSystem unitAnimationLaunchViewSystem,
            UnitsHealthBarUpdateSystem healthBarUpdateSystem,
            DeleteFrameEventSystem<DicesRollEventComponent> deleteRollEventSystem,
            DeleteFrameEventSystem<DiceLockEventComponent> diceLockEventSystem,
            DeleteFrameEventSystem<BattleSelectEventComponent> battleSelectEventSystem,
            DeleteFrameEventSystem<BattleDeselectEventComponent> battleDeselectEventSystem,
            DeleteFrameEventSystem<DiceAimingEventComponent> diceAimingEventSystem,
            DeleteFrameEventSystem<DiceUnaimingEventComponent> diceUnaimingEventSystem,
            DeleteFrameEventSystem<DiceTargetedEventComponent> diceTargetedEventSystem,
            DeleteFrameEventSystem<AnimationLaunchEventComponent> animationLaunchEventSystem,
            DeleteFrameEventSystem<AnimationActionReadyEventComponent> animationActionReadyEventSystem,
            DeleteFrameEventSystem<DeathEventComponent> deathEventSystem)
        {
            _battleDiceRollStateSystem = battleDiceRollStateSystem;
            _deathSystem = deathSystem;
            _battleTargetSelectStateSystem = battleTargetSelectStateSystem;
            _diceApplyStateSystem = diceApplyStateSystem;
            _battleUIUpdateSystem = battleUIUpdateSystem;
            _selectionViewSystem = selectionViewSystem;
            _diceApplySystem = diceApplySystem;
            _diceTargetSelectViewSystem = diceTargetSelectViewSystem;
            _damageSystem = damageSystem;
            _unitAnimationLaunchViewSystem = unitAnimationLaunchViewSystem;
            _healthBarUpdateSystem = healthBarUpdateSystem;
            _deleteRollEventSystem = deleteRollEventSystem;
            _diceLockEventSystem = diceLockEventSystem;
            _battleSelectEventSystem = battleSelectEventSystem;
            _battleDeselectEventSystem = battleDeselectEventSystem;
            _diceAimingEventSystem = diceAimingEventSystem;
            _diceUnaimingEventSystem = diceUnaimingEventSystem;
            _diceTargetedEventSystem = diceTargetedEventSystem;
            _animationLaunchEventSystem = animationLaunchEventSystem;
            _animationActionReadyEventSystem = animationActionReadyEventSystem;
            _deathEventSystem = deathEventSystem;
        }

        public void InitSystems(EcsSystems updateSystems)
        {
            updateSystems
                .Add(_battleDiceRollStateSystem)
                .Add(_battleTargetSelectStateSystem)
                .Add(_diceApplyStateSystem)
                .Add(_selectionViewSystem)
                .Add(_diceTargetSelectViewSystem)
                .Add(_diceApplySystem)
                .Add(_damageSystem)
                .Add(_deathSystem)
                .Add(_unitAnimationLaunchViewSystem)

                // Post update
                .Add(_battleUIUpdateSystem)
                .Add(_healthBarUpdateSystem)

                // Delete frame event systems
                .Add(_deleteRollEventSystem)
                .Add(_diceLockEventSystem)
                .Add(_battleSelectEventSystem)
                .Add(_battleDeselectEventSystem)
                .Add(_diceAimingEventSystem)
                .Add(_diceUnaimingEventSystem)
                .Add(_diceTargetedEventSystem)
                .Add(_animationLaunchEventSystem)
                .Add(_animationActionReadyEventSystem)
                .Add(_deathEventSystem)
                .Init();
        }
    }
}