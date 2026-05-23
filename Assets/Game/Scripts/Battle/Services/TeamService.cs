using CustomTypes;
using Infrastructure;
using Leopotam.EcsLite;
using Zenject;

namespace Battle
{
    public class TeamService
    {
        private readonly EcsService _ecsService;
        private readonly FrameComponentsService _frameComponentsService;
        
        private readonly EcsFilter _currentTeamFilter;
        private readonly EcsPool<TeamComponent> _teamPool;
        private readonly EcsPool<CurrentTeamComponent> _currentTeamPool;
        
        [Inject]
        public TeamService(EcsService ecsService, FrameComponentsService frameComponentsService)
        {
            _ecsService = ecsService;
            _frameComponentsService = frameComponentsService;

            _currentTeamFilter = ecsService.World.Filter<CurrentTeamComponent>().End();
            _currentTeamPool = ecsService.World.GetPool<CurrentTeamComponent>();
            _teamPool = ecsService.World.GetPool<TeamComponent>();
        }

        public void SetCurrentTeam(TeamType team)
        {
            foreach (var currentTeam in _currentTeamFilter)
            {
                ref var currentTeamComponent = ref _currentTeamPool.Get(currentTeam);
                if (currentTeamComponent.Team == team)
                    continue;
                
                currentTeamComponent.Team = team;
                _frameComponentsService.TryAddModifiedEvent<CurrentTeamComponent>(currentTeam);
            }
        }

        public TeamType GetTeam(int entity)
        {
            ref var teamComponent = ref _teamPool.Get(entity);
            return teamComponent.Team;
        }

        public bool IsCurrentTeamEntity(int entity)
            => IsCurrentTeam(GetTeam(entity));

        public bool IsCurrentTeam(TeamType team)
            => team == GetCurrentTeam();

        public bool IsTeamEntity(int entity, TeamType team)
            => GetTeam(entity) == team;

        private TeamType GetCurrentTeam()
        {
            foreach (var currentTeam in _currentTeamFilter)
            {
                ref var currentTeamComponent = ref _currentTeamPool.Get(currentTeam);
                return currentTeamComponent.Team;
            }
            
            return TeamType.None;
        }
    }
}