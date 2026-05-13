using System.Collections.Generic;
using Battle.Battlefield;
using CustomTypes;
using CustomTypes.Enums.Team;
using CustomTypes.Enums.Units;
using Infrastructure;
using Leopotam.EcsLite;
using Units.Factories;
using UnityEngine;
using Zenject;

namespace Battle
{
    public class TeamBuilder
    {
        private readonly EcsService _ecsService;
        private readonly UnitFactory _unitFactory;
        private readonly BattleCellService _cellService;
        private readonly DiceRollsConfig _rollsConfig;

        private readonly EcsPool<BattleLocationComponent> _battleLocationPool;
        private readonly EcsPool<TeamComponent> _teamPool;
        private readonly EcsPool<TeamBattleDicesRollComponent> _teamBattleDicesRollPool;

        private UnitSpecialization SpawningUnit => new(UnitClass.None, UnitArchetype.None, 1);
        private readonly Dictionary<TeamType, Vector2Int[]> _unitLocations = new()
        {
            [TeamType.Player] = new Vector2Int[]
            {
                new(1, 0),
                new(2, 0),
                new(1, 1),
                new(2, 1),
            },
            [TeamType.Enemy] = new Vector2Int[]
            {
                new(1, 2),
                new(2, 2),
                new(1, 3),
                new(2, 3),
            },
        };

        [Inject]
        public TeamBuilder(EcsService ecsService, UnitFactory unitFactory, BattleCellService cellService, DiceRollsConfig rollsConfig)
        {
            _ecsService = ecsService;
            _unitFactory = unitFactory;
            _cellService = cellService;
            _rollsConfig = rollsConfig;

            _teamBattleDicesRollPool = ecsService.World.GetPool<TeamBattleDicesRollComponent>();
            _teamPool = ecsService.World.GetPool<TeamComponent>();
            _battleLocationPool = ecsService.World.GetPool<BattleLocationComponent>();
        }

        public void BuildTeams()
        {
            BuildTeam(TeamType.Player);
            BuildTeam(TeamType.Enemy);
        }

        private void BuildTeam(TeamType team)
        {
            CreateTeamRolls(team);
            foreach (var location in _unitLocations[team])
                CreateUnit(location, team);
        }

        private void CreateTeamRolls(TeamType team)
        {
            var playerTeamRolls = _ecsService.CreateEntity();
            ref var playerTeamComponent = ref _teamPool.Add(playerTeamRolls);
            ref var playerTeamBattleDicesRollComponent = ref _teamBattleDicesRollPool.Add(playerTeamRolls);

            playerTeamComponent.Team = team;
            playerTeamBattleDicesRollComponent.StartRollsCount =
                team == TeamType.Player ? _rollsConfig.PlayerDefaultRollsCount : _rollsConfig.EnemyDefaultRollsCount;
        }

        private void CreateUnit(Vector2Int location, TeamType team)
        {
            var cell = _cellService.GetCell(location.x, location.y);
            var unit = _unitFactory.CreateUnit(SpawningUnit, team);

            ref var battleLocationComponent = ref _battleLocationPool.Get(unit);
            battleLocationComponent.Cell = cell;
            battleLocationComponent.Rotation = Quaternion.Euler(team == TeamType.Player ? Vector3.zero : new Vector3(0, 180, 0));
            cell.Occupier = _ecsService.World.PackEntity(unit);
        }
    }
}