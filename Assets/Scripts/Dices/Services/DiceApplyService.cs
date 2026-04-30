using Battle;
using CustomTypes;
using CustomTypes.Enums;
using CustomTypes.Enums.Infrastructure;
using CustomTypes.Enums.Team;
using Infrastructure;
using Leopotam.EcsLite;
using Zenject;

namespace Dices
{
    public class DiceApplyService
    {
        public bool IsApplyingDice { get; private set; }

        private readonly TeamService _teamService;
        private readonly DamageService _damageService;
        private readonly HealthService _healthService;

        private readonly EcsFilter _targetedFilter;

        [Inject]
        public DiceApplyService(EcsService ecsService, TeamService teamService, DamageService damageService, HealthService healthService)
        {
            _teamService = teamService;
            _damageService = damageService;
            _healthService = healthService;

            _targetedFilter = ecsService.World.Filter<TargetedComponent>().Exc<DeadComponent>().End();
        }

        public void StartDiceApplying()
        {
            _teamService.SetCurrentTeam(TeamType.Player);
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

        public void ApplyDiceSide(DiceSide diceSide, int targeted)
        {
            switch (diceSide.SideType)
            {
                case DiceSideType.MeleeAttack:
                case DiceSideType.RangeAttack:
                    _damageService.Damage(targeted, diceSide.Value);
                    break;
                case DiceSideType.Armor:
                    _healthService.IncreaseArmor(targeted, diceSide.Value);
                    break;
            }
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