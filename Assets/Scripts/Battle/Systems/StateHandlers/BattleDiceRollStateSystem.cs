using Leopotam.EcsLite;
using Zenject;

namespace Battle
{
    public class BattleDiceRollStateSystem : IEcsRunSystem
    {
        private readonly BattleStateMachine _battleStateMachine;
        private readonly BattleDiceRollService _rollService;
        private readonly TeamService _teamService;

        [Inject]
        public BattleDiceRollStateSystem(BattleStateMachine battleStateMachine, BattleDiceRollService rollService, TeamService teamService)
        {
            _battleStateMachine = battleStateMachine;
            _rollService = rollService;
            _teamService = teamService;
        }

        public void Run(IEcsSystems systems)
        {
            if (!_rollService.IsRollingState)
                return;

            if (IsRollingStateCompleted())
            {
                _battleStateMachine.Enter<TargetSelectState>();
                return;
            }

            if (IsEnemyRollingComplete())
                _rollService.StartTeamDiceRolling(_rollService.RollingEndTeam);
        }

        private bool IsRollingStateCompleted()
            => _teamService.IsCurrentTeam(_rollService.RollingEndTeam) && _rollService.IsCurrentTeamRollingFinished();

        private bool IsEnemyRollingComplete()
            => _teamService.IsCurrentTeam(_rollService.RollingStartTeam) && _rollService.IsCurrentTeamRollingFinished();
    }
}