using System;
using CustomTypes;
using Infrastructure;
using Leopotam.EcsLite;
using Utils;
using Zenject;

namespace Battle
{
    public class HealthService
    {
        private readonly BattleDeathService _battleDeathService;

        private readonly EcsPool<ArmorComponent> _armorPool;
        private readonly EcsPool<HealthComponent> _healthPool;

        [Inject]
        public HealthService(EcsService ecsService, BattleDeathService battleDeathService)
        {
            _battleDeathService = battleDeathService;

            _armorPool = ecsService.World.GetPool<ArmorComponent>();
            _healthPool = ecsService.World.GetPool<HealthComponent>();
        }

        public void IncreaseArmor(int entity, int armor)
        {
            ref var armorComponent = ref _armorPool.GetOrAdd(entity);
            armorComponent.Armor += armor;
        }

        public bool TryReduceArmor(int entity, int damage, out int damageLeft)
        {
            if (!_armorPool.Has(entity))
            {
                damageLeft = damage;
                return false;
            }

            ref var armorComponent = ref _armorPool.Get(entity);
            armorComponent.Armor -= damage;
            if (armorComponent.Armor <= 0)
            {
                damageLeft = Math.Abs(armorComponent.Armor);
                _armorPool.Del(entity);
            }
            else
                damageLeft = 0;

            return true;
        }

        public void ReduceHealth(int entity, int damage)
        {
            ref var healthComponent = ref _healthPool.Get(entity);
            healthComponent.Hp -= damage;

            if (healthComponent.Hp <= 0)
                _battleDeathService.Die(entity);
            
            LogService.LogDebug(DebugType.Log, $"Unit {entity}: Hp {healthComponent.Hp}");
        }

        public float GetCurrentHp(int unit)
        {
            ref var healthComponent = ref _healthPool.Get(unit);
            return healthComponent.Hp;
        }

        public int GetMaxHp(int unit)
        {
            ref var healthComponent = ref _healthPool.Get(unit);
            return healthComponent.MaxHp;
        }

        public int GetArmor(int unit)
        {
            if (!_armorPool.Has(unit))
                return 0;
            
            ref var armorComponent = ref _armorPool.Get(unit);
            return armorComponent.Armor;
        }
    }
}