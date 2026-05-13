using System.Collections.Generic;
using CustomTypes.Enums.Team;
using Infrastructure;
using Leopotam.EcsLite;
using Units;
using Zenject;

namespace Battle
{
    public class BattleDeathService
    {
        private readonly UnitService _unitService;
        private readonly TeamService _teamService;

        private readonly EcsFilter _aliveUnitFilter;
        private readonly EcsPool<DeadComponent> _deadPool;
        private readonly EcsPool<DeathEventComponent> _deathEventPool;

        [Inject]
        public BattleDeathService(EcsService ecsService, UnitService unitService, TeamService teamService)
        {
            _unitService = unitService;
            _teamService = teamService;

            _aliveUnitFilter = ecsService.World.Filter<UnitComponent>().
                Exc<DeadComponent>().End();
            _deadPool = ecsService.World.GetPool<DeadComponent>();
            _deathEventPool = ecsService.World.GetPool<DeathEventComponent>();
        }
        
        public void Die(int unit)
        {
            _deathEventPool.Add(unit);
            _deadPool.Add(unit);
            
            if (_unitService.TryGetMainDice(unit, out var dice))
            {
                _deathEventPool.Add(dice);
                _deadPool.Add(dice);
            }
        }

        public bool IsDead(int target)
            => _deadPool.Has(target);

        public bool IsAnyTeamDead()
        {
            var aliveTeams = new HashSet<TeamType>();
            foreach (var unit in _aliveUnitFilter)
            {
                var team = _teamService.GetTeam(unit);
                aliveTeams.Add(team);

                if (aliveTeams.Count > 1)
                    return false;
            }

            return true;
        }

        public TeamType GetAnyAliveTeam()
        {
            foreach (var unit in _aliveUnitFilter)
                return _teamService.GetTeam(unit);

            return TeamType.None;
        }
    }
}