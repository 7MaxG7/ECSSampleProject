using Battle;
using Battle.Battlefield;
using CustomTypes;
using Cysharp.Threading.Tasks;
using Infrastructure;
using Leopotam.EcsLite;
using UI.Units;
using UnityEngine;

namespace Units
{
    public class UnitSpawner
    {
        private readonly UnitViewFactory _unitViewFactory;
        private readonly BattlefieldViewService _battlefieldViewService;
        private readonly HighlightService _highlightService;
        private readonly UnitOverlayUIService _unitOverlayUIService;

        private readonly EcsPool<UnitComponent> _unitPool;
        private readonly EcsPool<BattleLocationComponent> _battleLocationPool;
        private readonly EcsPool<UnitViewComponent> _unitViewPool;
        private readonly EcsPool<TeamComponent> _teamPool;

        public UnitSpawner(EcsService ecsService, UnitViewFactory unitViewFactory, BattlefieldViewService battlefieldViewService,
            HighlightService highlightService, UnitOverlayUIService unitOverlayUIService)
        {
            _unitViewFactory = unitViewFactory;
            _battlefieldViewService = battlefieldViewService;
            _highlightService = highlightService;
            _unitOverlayUIService = unitOverlayUIService;

            _unitPool = ecsService.World.GetPool<UnitComponent>();
            _battleLocationPool = ecsService.World.GetPool<BattleLocationComponent>();
            _unitViewPool = ecsService.World.GetPool<UnitViewComponent>();
            _teamPool = ecsService.World.GetPool<TeamComponent>();
        }

        public async UniTask SpawnUnitAsync(int unit)
        {
            var spawnParams = GetSpawnParams(unit);
            var position = _battlefieldViewService.GetTilePosition(spawnParams.Cell);

            await _unitViewFactory.SpawnUnitAsync(unit, spawnParams.Specialization, position, spawnParams.Rotation, spawnParams.Team);
        }

        public void DespawnUnit(int unit)
        {
            _unitViewPool.Del(unit);
            _highlightService.Clear(unit);
            _unitOverlayUIService.Clear(unit);
        }

        private (UnitSpecialization Specialization, BattleCell Cell, Quaternion Rotation, TeamType Team) GetSpawnParams(int unit)
        {
            ref var unitComponent = ref _unitPool.Get(unit);
            ref var battleLocationComponent = ref _battleLocationPool.Get(unit);
            ref var teamComponent = ref _teamPool.Get(unit);

            return (unitComponent.Specialization, battleLocationComponent.Cell, battleLocationComponent.Rotation, teamComponent.Team);
        }
    }
}