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

        private readonly EcsFilter _deadUnitAddedEventFilter;

        [Inject]
        public DeathSystem(EcsService ecsService, DiceTargetSelectService diceTargetSelectService,
            BattleLocationService battleLocationService, FrameComponentsService frameComponentsService)
        {
            _diceTargetSelectService = diceTargetSelectService;
            _battleLocationService = battleLocationService;

            _deadUnitAddedEventFilter = frameComponentsService.GetAddedEventMask<DeadComponent>().Inc<UnitComponent>().End();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var unit in _deadUnitAddedEventFilter)
            {
                _diceTargetSelectService.ClearUnitSelections(unit);
                _battleLocationService.ClearUnitLocation(unit);
            }
        }
    }
}