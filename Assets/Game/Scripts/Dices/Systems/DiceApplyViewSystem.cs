using Cysharp.Threading.Tasks;
using Infrastructure;
using Leopotam.EcsLite;
using Zenject;

namespace Dices
{
    public class DiceApplyViewSystem : IEcsRunSystem
    {
        private readonly EcsService _ecsService;
        private readonly DiceApplyViewService _diceApplyViewService;
        private readonly BattleDiceService _battleDiceService;
        private readonly CancellationTokenProvider _tokenProvider;

        private readonly EcsFilter _diceAppliedEventFilter;
        private readonly EcsPool<DiceAppliedEventComponent> _diceAppliedEventPool;

        [Inject]
        public DiceApplyViewSystem(EcsService ecsService, FrameComponentsService frameComponentsService,
            BattleDiceService battleDiceService, DiceApplyViewService diceApplyViewService, CancellationTokenProvider tokenProvider)
        {
            _ecsService = ecsService;
            _diceApplyViewService = diceApplyViewService;
            _battleDiceService = battleDiceService;
            _tokenProvider = tokenProvider;

            _diceAppliedEventFilter = frameComponentsService.GetEventFilter<DiceAppliedEventComponent>();
            _diceAppliedEventPool = ecsService.World.GetPool<DiceAppliedEventComponent>();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var dice in _diceAppliedEventFilter)
                AnimateDiceApplyAsync(dice).Forget();
        }

        private async UniTaskVoid AnimateDiceApplyAsync(int dice)
        {
            if (!TryGetDiceTarget(dice, out var target))
                return;

            using var localCts = _tokenProvider.CreateLocalCts();
            _battleDiceService.TryGetOwner(dice, out var unit);
            var diceSide = _battleDiceService.GetCurrentSide(dice);
            await _diceApplyViewService.AnimateDiceApplyAsync(unit, target, diceSide.SideType, localCts);
        }

        private bool TryGetDiceTarget(int dice, out int target)
        {
            ref var diceAppliedEventComponent = ref _diceAppliedEventPool.Get(dice);
            return _ecsService.TryUnpack(diceAppliedEventComponent.Target, out target);
        }
    }
}