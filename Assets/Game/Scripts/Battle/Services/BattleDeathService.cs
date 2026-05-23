using System.Collections.Generic;
using CustomTypes;
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
        private readonly FrameComponentsService _frameComponentsService;

        private readonly EcsFilter _aliveUnitFilter;
        private readonly EcsPool<DeadComponent> _deadPool;

        [Inject]
        public BattleDeathService(EcsService ecsService, UnitService unitService, FrameComponentsService frameComponentsService,
            TeamService teamService)
        {
            _unitService = unitService;
            _teamService = teamService;
            _frameComponentsService = frameComponentsService;

            _aliveUnitFilter = ecsService.World.Filter<UnitComponent>().Exc<DeadComponent>().End();
            _deadPool = ecsService.World.GetPool<DeadComponent>();
        }

        public void Die(int unit)
        {
            _deadPool.Add(unit);
            _frameComponentsService.AddAddedEvent<DeadComponent>(unit);

            if (_unitService.TryGetDice(unit, out var dice))
            {
                _deadPool.Add(dice);
                _frameComponentsService.AddAddedEvent<DeadComponent>(dice);
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