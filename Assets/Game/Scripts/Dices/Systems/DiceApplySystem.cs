using Battle;
using CustomTypes;
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
        private readonly DiceTargetSelectService _targetSelectService;
        private readonly DiceApplyService _diceApplyService;
        private readonly DiceApplyViewService _diceApplyViewService;

        private readonly EcsFilter _targetedFilter;
        private readonly EcsPool<TargetedComponent> _targetedPool;

        [Inject]
        public DiceApplySystem(EcsService ecsService, DiceApplyService diceApplyService, DiceApplyViewService diceApplyViewService,
            BattleDiceService battleDiceService, DiceTargetSelectService targetSelectService, TeamService teamService)
        {
            _ecsService = ecsService;
            _teamService = teamService;
            _battleDiceService = battleDiceService;
            _targetSelectService = targetSelectService;
            _diceApplyService = diceApplyService;
            _diceApplyViewService = diceApplyViewService;

            _targetedFilter = ecsService.World.Filter<TargetedComponent>().End();
            _targetedPool = ecsService.World.GetPool<TargetedComponent>();
        }

        public void Run(IEcsSystems systems)
        {
            if (!_diceApplyService.IsApplyingDice || _diceApplyViewService.IsInProgress)
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

                    ApplyDiceSide(targeted, dice);
                    var diceSide = _battleDiceService.GetCurrentSide(dice);
                    LogService.LogDebug(DebugType.Log, $"Dice {dice}: {diceSide.SideType}-{diceSide.Value} to {targeted}");
                    return;
                }
            }
        }

        private void ApplyDiceSide(int target, int dice)
        {
            _diceApplyService.ApplyDiceSide(dice, target);
            _targetSelectService.ClearDiceTarget(dice);
        }
    }
}