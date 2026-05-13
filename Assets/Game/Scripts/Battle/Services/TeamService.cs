using CustomTypes.Enums.Team;
using Infrastructure;
using Leopotam.EcsLite;
using Zenject;

namespace Battle
{
    public class TeamService
    {
        private readonly EcsPool<TeamComponent> _teamPool;
        
        private TeamType _currentTeam;
        
        [Inject]
        public TeamService(EcsService ecsService)
        {
            _teamPool = ecsService.World.GetPool<TeamComponent>();
        }
        
        public void SetCurrentTeam(TeamType team)
        {
            _currentTeam = team;
        }

        public TeamType GetTeam(int entity)
        {
            ref var teamComponent = ref _teamPool.Get(entity);
            return teamComponent.Team;
        }

        public bool IsCurrentTeamEntity(int entity)
            => IsCurrentTeam(GetTeam(entity));

        public bool IsCurrentTeam(TeamType team)
            => team == _currentTeam;
    }
}