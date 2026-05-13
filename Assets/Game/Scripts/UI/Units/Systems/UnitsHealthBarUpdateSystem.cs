using Battle;
using CustomTypes.Enums;
using Dices;
using Infrastructure;
using Leopotam.EcsLite;
using UI.Battle;
using Units;
using Utils.Extensions;
using Zenject;

namespace UI.Units
{
    /// <summary>
    /// System that observes changes in units health and incoming damage to send this data to ui overlay
    /// </summary>
    public class UnitsHealthBarUpdateSystem : IEcsPostRunSystem
    {
        private readonly EcsService _ecsService;
        private readonly BattleDiceService _battleDiceService;
        private readonly HealthService _healthService;
        private readonly BattleUIController _battleUIController;

        private readonly EcsFilter _undelayedUnitUIOverlayFilter;
        private readonly EcsPool<TargetedComponent> _targetedPool;

        [Inject]
        public UnitsHealthBarUpdateSystem(EcsService ecsService, BattleDiceService battleDiceService, HealthService healthService,
            BattleUIController battleUIController)
        {
            _ecsService = ecsService;
            _battleDiceService = battleDiceService;
            _healthService = healthService;
            _battleUIController = battleUIController;

            _undelayedUnitUIOverlayFilter = ecsService.World.Filter<UnitUIOverlayComponent>().
                Exc<ViewUpdateDelayComponent>().End();
            _targetedPool = ecsService.World.GetPool<TargetedComponent>();
        }

        public void PostRun(IEcsSystems systems)
        {
            foreach (var unitOverlay in _undelayedUnitUIOverlayFilter)
            {
                _battleUIController.UpdateHealthBar(unitOverlay);
                UpdateIncomingDamage(unitOverlay);
            }
        }

        private void UpdateIncomingDamage(int target)
        {
            var damage = 0;
            var armor = 0;

            if (_targetedPool.Has(target))
            {
                ref var targetedComponent = ref _targetedPool.Get(target);
                foreach (var dicePacked in targetedComponent.TargetedDices)
                {
                    if (!_ecsService.TryUnpackWithWarning(dicePacked, out var targetedDice))
                        continue;

                    var currentSide = _battleDiceService.GetCurrentSide(targetedDice);
                    if (currentSide.SideType.IsDamageSide())
                    {
                        if (currentSide.Value >= armor)
                        {
                            damage += currentSide.Value - armor;
                            armor = 0;
                        }
                        else
                            armor -= currentSide.Value;

                        var unitHealth = _healthService.GetCurrentHp(target).Ceiling() + _healthService.GetArmor(target);
                        if (unitHealth <= damage)
                        {
                            damage = unitHealth;
                            break;
                        }
                    }
                    else if (currentSide.SideType is DiceSideType.Armor)
                        armor += currentSide.Value;
                }
            }

            _battleUIController.UpdateIncomingDamage(target, damage);
        }
    }
}