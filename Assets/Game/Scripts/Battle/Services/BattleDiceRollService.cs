using CustomTypes;
using Dices;
using Infrastructure;
using Leopotam.EcsLite;
using Zenject;

namespace Battle
{
    public class BattleDiceRollService
    {
        public TeamType RollingStartTeam => TeamType.Enemy;
        public TeamType RollingEndTeam => TeamType.Player;
        public bool IsRollingState { get; private set; }
        
        private readonly RandomService _random;
        private readonly TeamService _teamService;
        private readonly DiceViewService _diceViewService;
        private readonly BattleDiceLockService _lockService;

        private readonly EcsFilter _unlockedDiceFilter;
        private readonly EcsFilter _teamRollFilter;
        private readonly EcsPool<DiceComponent> _dicePool;
        private readonly EcsPool<TeamBattleDicesRollComponent> _teamBattleDicesRollPool;
        private readonly EcsPool<DicesRollEventComponent> _dicesRollEventPool;

        [Inject]
        public BattleDiceRollService(EcsService ecsService, RandomService random, TeamService teamService, DiceViewService diceViewService,
            BattleDiceLockService lockService)
        {
            _random = random;
            _teamService = teamService;
            _diceViewService = diceViewService;
            _lockService = lockService;

            _teamRollFilter = ecsService.World.Filter<TeamBattleDicesRollComponent>().End();
            _unlockedDiceFilter = ecsService.World.Filter<DiceComponent>().Exc<LockedComponent>().Exc<DeadComponent>().End();
            _dicePool = ecsService.World.GetPool<DiceComponent>();
            _teamBattleDicesRollPool = ecsService.World.GetPool<TeamBattleDicesRollComponent>();
            _dicesRollEventPool = ecsService.World.GetPool<DicesRollEventComponent>();
        }

        public void StartDiceRolling()
        {
            foreach (var teamRoll in _teamRollFilter)
            {
                ref var teamBattleDicesRollComponent = ref _teamBattleDicesRollPool.Get(teamRoll);
                teamBattleDicesRollComponent.RollsLeft = teamBattleDicesRollComponent.StartRollsCount;
            }

            StartTeamDiceRolling(RollingStartTeam);
            IsRollingState = true;
        }

        public void StartTeamDiceRolling(TeamType team)
        {
            _teamService.SetCurrentTeam(team);
            _diceViewService.ActivateCurrentTeamDices();
            _lockService.SetCurrentTeamDicesLock(false);
            RollCurrentTeamUnlockedMainDices();
            LogService.LogDebug(DebugType.Log, $"{team}'s turn");
        }

        public void FinishDiceRolling()
        {
            _teamService.SetCurrentTeam(TeamType.None);
            IsRollingState = false;
        }

        public void RollCurrentTeamUnlockedMainDices()
        {
            foreach (var dice in _unlockedDiceFilter)
            {
                if (!_teamService.IsCurrentTeamEntity(dice))
                    continue;

                ref var diceComponent = ref _dicePool.Get(dice);
                diceComponent.CurrentSide = _random.GetRandom(diceComponent.Config.Sides);
            }

            DecreaseCurrentTeamRollsCount();
        }

        public bool IsCurrentTeamRollingFinished()
        {
            if (!TryGetCurrentTeamRoll(out var teamRoll))
                return true;

            ref var teamBattleDicesRollComponent = ref _teamBattleDicesRollPool.Get(teamRoll);
            return IsTeamRollingFinished(in teamBattleDicesRollComponent);
        }

        private void DecreaseCurrentTeamRollsCount()
        {
            if (!TryGetCurrentTeamRoll(out var teamRoll))
                return;

            ref var teamBattleDicesRollComponent = ref _teamBattleDicesRollPool.Get(teamRoll);
            --teamBattleDicesRollComponent.RollsLeft;

            if (IsTeamRollingFinished(in teamBattleDicesRollComponent))
                _lockService.SetCurrentTeamDicesLock(true);
            else if (_lockService.AreCurrentTeamDicesLocked())
                FinishTeamRolling(ref teamBattleDicesRollComponent);

            _dicesRollEventPool.Add(teamRoll);
        }

        private bool TryGetCurrentTeamRoll(out int teamRolls)
        {
            foreach (var teamRoll in _teamRollFilter)
                if (_teamService.IsCurrentTeamEntity(teamRoll))
                {
                    teamRolls = teamRoll;
                    return true;
                }

            LogService.LogDebug(DebugType.Warning, "Cannot find team rolls for current team");
            teamRolls = -1;
            return false;
        }

        private bool IsTeamRollingFinished(in TeamBattleDicesRollComponent teamBattleDicesRollComponent)
            => teamBattleDicesRollComponent.RollsLeft <= 0;

        private void FinishTeamRolling(ref TeamBattleDicesRollComponent teamBattleDicesRollComponent)
            => teamBattleDicesRollComponent.RollsLeft = 0;
    }
}