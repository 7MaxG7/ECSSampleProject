using System.Collections.Generic;
using System.Linq;
using CustomTypes;
using Infrastructure;
using Leopotam.EcsLite;
using UnityEngine;
using UnityEngine.Tilemaps;
using Utils;

namespace Battle.Battlefield
{
    public class BattlefieldViewService
    {
        private readonly EcsFilter _battlefieldViewFilter;
        private readonly EcsPool<BattlefieldViewComponent> _battlefieldViewPool;
        
        private readonly Dictionary<TileState, TileBase> _stateTiles;

        public BattlefieldViewService(EcsService ecsService, BattlefieldViewConfig battlefieldViewConfig)
        {
            _battlefieldViewFilter = ecsService.World.Filter<BattlefieldViewComponent>().End();
            _battlefieldViewPool = ecsService.World.GetPool<BattlefieldViewComponent>();

            _stateTiles = battlefieldViewConfig.StateTiles.ToDictionary(data => data.State, data => data.Tile);
        }

        public void SetTile(Vector3Int position, TileState state)
        {
            if (!_stateTiles.TryGetValue(state, out var tile))
            {
                LogService.LogDebug(DebugType.Warning, $"No tile for state {state}");
                return;
            }
            
            GetBattlefieldView().SetTile(position, tile);
        }

        public Vector3 GetTilePosition(BattleCell cell)
            => GetBattlefieldView().Grid.GetCellCenterWorld(cell.Location);

        private BattlefieldView GetBattlefieldView()
        {
            var battlefieldView = _battlefieldViewFilter.GetSingle();
            ref var battlefieldViewComponent = ref _battlefieldViewPool.Get(battlefieldView);
            return battlefieldViewComponent.BattlefieldView;
        }
    }
}