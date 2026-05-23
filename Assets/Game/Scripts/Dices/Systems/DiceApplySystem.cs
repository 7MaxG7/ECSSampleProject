using Battle;
using CustomTypes;
using Cysharp.Threading.Tasks;
using Infrastructure;
using Leopotam.EcsLite;
using Zenject;

namespace Dices
{
    public class DiceApplySystem : IEcsRunSystem
    {
        private readonly EcsService _ecsService;
        private readonly TeamService _teamService;
        private readonly BattleDiceService _battleDiceService;
        private readonly CancellationTokenProvider _tokenProvider;
        private readonly DiceTargetSelectService _targetSelectService;
        private readonly DiceApplyService _diceApplyService;
        private readonly DiceApplyViewService _diceApplyViewService;

        private readonly EcsFilter _targetedFilter;
        private readonly EcsPool<TargetedComponent> _targetedPool;

        private bool _isInProcess;

        [Inject]
        public DiceApplySystem(EcsService ecsService, DiceApplyService diceApplyService, DiceApplyViewService diceApplyViewService,
            BattleDiceService battleDiceService, CancellationTokenProvider tokenProvider, DiceTargetSelectService targetSelectService,
            TeamService teamService)
        {
            _ecsService = ecsService;
            _teamService = teamService;
            _battleDiceService = battleDiceService;
            _tokenProvider = tokenProvider;
            _targetSelectService = targetSelectService;
            _diceApplyService = diceApplyService;
            _diceApplyViewService = diceApplyViewService;

            _targetedFilter = ecsService.World.Filter<TargetedComponent>().End();
            _targetedPool = ecsService.World.GetPool<TargetedComponent>();
        }

        public void Run(IEcsSystems systems)
        {
            if (!_diceApplyService.IsApplyingDice || _isInProcess)
                return;

            foreach (var targeted in _targetedFilter)
            {
                if (_teamService.IsCurrentTeamEntity(targeted))
                    continue;

                ref var targetedComponent = ref _targetedPool.Get(targeted);
                while (targetedComponent.TargetedDices.TryPeek(out var dicePacked))
                {
                    if (!_ecsService.TryUnpack(dicePacked, out var dice))
                    {
                        targetedComponent.TargetedDices.Pop();
                        continue;
                    }

                    _battleDiceService.TryGetOwner(dice, out var unit);
                    ProcessDicesApply(unit, targeted, dice).Forget();
                    return;
                }
            }
        }

        private async UniTaskVoid ProcessDicesApply(int unit, int target, int dice)
        {
            using var localCts = _tokenProvider.CreateLocalCts();
            _isInProcess = true;

            ApplyDiceSide(target, dice, out var diceSide);
            await _diceApplyViewService.AnimateDiceApplyAsync(unit, target, diceSide.SideType, localCts);

            LogService.LogDebug(DebugType.Log, $"Unit {unit}: {diceSide.SideType}-{diceSide.Value} to {target}");
            while (_diceApplyService.IsApplyInProgress(target, diceSide.SideType))
                await UniTask.NextFrame(localCts.Token);

            _isInProcess = false;
        }

        private void ApplyDiceSide(int target, int dice, out DiceSide diceSide)
        {
            diceSide = _battleDiceService.GetCurrentSide(dice);
            _diceApplyService.ApplyDiceSide(diceSide, target);
            _targetSelectService.ClearDiceTarget(dice);
        }
    }
}