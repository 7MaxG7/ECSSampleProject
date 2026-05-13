using Dices;
using Infrastructure;
using Leopotam.EcsLite;
using Units;
using Zenject;

namespace Battle
{
    public class DeathSystem : IEcsRunSystem
    {
        private readonly DiceTargetSelectService _diceTargetSelectService;
        private readonly BattleLocationService _battleLocationService;

        private readonly EcsFilter _diedUnitsFilter;

        [Inject]
        public DeathSystem(EcsService ecsService, DiceTargetSelectService diceTargetSelectService,
            BattleLocationService battleLocationService)
        {
            _diceTargetSelectService = diceTargetSelectService;
            _battleLocationService = battleLocationService;

            _diedUnitsFilter = ecsService.World.Filter<UnitComponent>().Inc<DeathEventComponent>().End();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var unit in _diedUnitsFilter)
            {
                _diceTargetSelectService.ClearUnitSelections(unit);
                _battleLocationService.ClearUnitLocation(unit);
            }
        }
    }
}