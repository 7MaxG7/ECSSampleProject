using Infrastructure;
using Leopotam.EcsLite;
using Zenject;

namespace Dices
{
    public class DiceFactory
    {
        private readonly EcsService _ecsService;

        private readonly EcsPool<DiceComponent> _dicePool;

        [Inject]
        public DiceFactory(EcsService ecsService)
        {
            _ecsService = ecsService;

            _dicePool = ecsService.World.GetPool<DiceComponent>();
        }

        public int CreateDice(int unit, DiceConfig config)
        {
            var dice = _ecsService.CreateEntity();
            InitComponents(unit, config, dice);
            return dice;
        }

        private void InitComponents(int unit, DiceConfig config, int dice)
        {
            ref var diceComponent = ref _dicePool.Add(dice);
            diceComponent.CurrentSide = config.Sides[0];
            diceComponent.Unit = _ecsService.World.PackEntity(unit);
            diceComponent.Config = config;
        }
    }
}