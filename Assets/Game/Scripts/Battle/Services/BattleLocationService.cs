using System;
using Battle.Battlefield;
using Infrastructure;
using Leopotam.EcsLite;
using UnityEngine;
using Zenject;

namespace Battle
{
    public class BattleLocationService
    {
        private readonly EcsService _ecsService;
        private readonly BattleCellService _battleCellService;

        private readonly EcsPool<BattleLocationComponent> _battleLocationPool;

        [Inject]
        public BattleLocationService(EcsService ecsService, BattleCellService battleCellService)
        {
            _ecsService = ecsService;
            _battleCellService = battleCellService;

            _battleLocationPool = ecsService.World.GetPool<BattleLocationComponent>();
        }

        public bool IsObjectBetweenUnitsExists(int target, int unit, Func<int, bool> condition)
        {
            var unitLocation = GetLocation(unit);
            var targetLocation = GetLocation(target);
            if (Math.Abs(unitLocation.y - targetLocation.y) <= 1)
                return false;

            var minLineY = Math.Min(unitLocation.y, targetLocation.y);
            var maxLineY = Math.Max(unitLocation.y, targetLocation.y);
            for (var lineY = minLineY + 1; lineY < maxLineY; lineY++)
            {
                foreach (var cell in _battleCellService.GetCellsAtY(lineY))
                    if (_ecsService.TryUnpack(cell.Occupier, out var occupier) && condition(occupier))
                        return true;
            }

            return false;
        }

        public void ClearUnitLocation(int unit)
        {
            ref var battleLocationComponent = ref _battleLocationPool.Get(unit);
            battleLocationComponent.Cell.Occupier = null;
        }

        private Vector3Int GetLocation(int entity)
        {
            ref var battleLocationComponent = ref _battleLocationPool.Get(entity);
            return battleLocationComponent.Cell.Location;
        }
    }
}