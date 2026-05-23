using Battle;
using Battle.Battlefield;
using CustomTypes;
using Dices;
using Infrastructure;
using Leopotam.EcsLite;

namespace Units
{
    public class UnitFactory
    {
        private readonly EcsService _ecsService;
        private readonly StaticDataService _dataService;
        private readonly DiceFactory _diceFactory;

        private readonly EcsPool<UnitComponent> _unitPool;
        private readonly EcsPool<BattleLocationComponent> _battleLocationPool;
        private readonly EcsPool<TeamComponent> _teamPool;
        private readonly EcsPool<HealthComponent> _healthPool;

        public UnitFactory(EcsService ecsService, StaticDataService dataService, DiceFactory diceFactory)
        {
            _ecsService = ecsService;
            _dataService = dataService;
            _diceFactory = diceFactory;

            _unitPool = _ecsService.World.GetPool<UnitComponent>();
            _battleLocationPool = _ecsService.World.GetPool<BattleLocationComponent>();
            _teamPool = _ecsService.World.GetPool<TeamComponent>();
            _healthPool = _ecsService.World.GetPool<HealthComponent>();
        }

        public int CreateUnit(UnitSpecialization specialization, TeamType team)
        {
            var config = _dataService.GetAnyUnit(specialization);
            if (config == null)
            {
                LogService.LogDebug(DebugType.Error, $"Cannot get unit config for specialization {specialization}");
                return -1;
            }

            var unit = _ecsService.CreateEntity();

            ref var unitComponent = ref _unitPool.Add(unit);
            unitComponent.Id = $"{config.Id}_{unit}";
            unitComponent.Specialization = specialization;
            unitComponent.Dice = _ecsService.World.PackEntity(CreateMainDice(unit, team, config));
            
            ref var healthComponent = ref _healthPool.Add(unit);
            healthComponent.MaxHp = config.Hp;
            healthComponent.Hp = config.Hp;
            
            ref var teamComponent = ref _teamPool.Add(unit);
            teamComponent.Team = team;

            _battleLocationPool.Add(unit);
            
            return unit;
        }

        private int CreateMainDice(int unit, TeamType team, UnitConfig config)
        {
            var mainDice = _diceFactory.CreateDice(unit, config.Dice);
            
            ref var teamComponent = ref _teamPool.Add(mainDice);
            teamComponent.Team = team;
            
            return mainDice;
        }
    }
}