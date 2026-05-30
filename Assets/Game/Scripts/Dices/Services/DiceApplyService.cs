using Battle;
using CustomTypes;
using Infrastructure;
using Leopotam.EcsLite;
using Zenject;

namespace Dices
{
    public class DiceApplyService
    {
        public TeamType ApplyStartTeam => TeamType.Player;
        public TeamType ApplyEndTeam => TeamType.Enemy;
        public bool IsApplyingDice { get; private set; }

        private readonly EcsService _ecsService;
        private readonly TeamService _teamService;
        private readonly DamageService _damageService;
        private readonly HealthService _healthService;
        private readonly FrameComponentsService _frameComponentsService;
        private readonly BattleDiceService _battleDiceService;

        private readonly EcsFilter _targetedFilter;

        [Inject]
        public DiceApplyService(EcsService ecsService, TeamService teamService, DamageService damageService, HealthService healthService,
            FrameComponentsService frameComponentsService, BattleDiceService battleDiceService)
        {
            _ecsService = ecsService;
            _teamService = teamService;
            _damageService = damageService;
            _healthService = healthService;
            _frameComponentsService = frameComponentsService;
            _battleDiceService = battleDiceService;

            _targetedFilter = ecsService.World.Filter<TargetedComponent>().Exc<DeadComponent>().End();
        }

        public void StartDiceApplying()
        {
            _teamService.SetCurrentTeam(ApplyStartTeam);
            IsApplyingDice = true;

            LogService.LogDebug(DebugType.Log, "Player's turn");
        }

        public void FinishDiceApplying()
        {
            _teamService.SetCurrentTeam(TeamType.None);
            IsApplyingDice = false;
        }

        public bool IsCurrentTeamDiceApplyingFinished()
        {
            foreach (var targeted in _targetedFilter)
                if (!_teamService.IsCurrentTeamEntity(targeted))
                    return false;

            return !IsAnyApplyInProgress();
        }

        public void ApplyDiceSide(int dice, int targeted)
        {
            var diceSide = _battleDiceService.GetCurrentSide(dice);
            switch (diceSide.SideType)
            {
                case DiceSideType.MeleeAttack:
                case DiceSideType.RangeAttack:
                    _damageService.Damage(targeted, diceSide.Value);
                    break;
                
                case DiceSideType.Armor:
                    _healthService.IncreaseArmor(targeted, diceSide.Value);
                    break;
                
                default:
                    return;
            }

            ref var diceAppliedEventComponent = ref _frameComponentsService.AddEvent<DiceAppliedEventComponent>(dice);
            diceAppliedEventComponent.Target = _ecsService.World.PackEntity(targeted);
        }

        public bool IsApplyInProgress(int target, DiceSideType sideType)
        {
            switch (sideType)
            {
                case DiceSideType.MeleeAttack:
                case DiceSideType.RangeAttack:
                    return _damageService.IsDamaging(target);
                default:
                    return false;
            }
        }

        private bool IsAnyApplyInProgress()
            => _damageService.IsDamagingAnyone();
    }
}