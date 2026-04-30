using CustomTypes;
using CustomTypes.Enums.Infrastructure;
using Infrastructure;
using Leopotam.EcsLite;
using Utils.Extensions;

namespace Battle.Battlefield
{
    public class BattleCellService
    {
        private readonly BattlefieldConfig _battlefieldConfig;
        private readonly EcsFilter _battlefieldFilter;
        private readonly EcsPool<BattlefieldComponent> _battlefieldPool;

        public BattleCellService(EcsService ecsService, BattlefieldConfig battlefieldConfig)
        {
            _battlefieldConfig = battlefieldConfig;
            
            _battlefieldFilter = ecsService.World.Filter<BattlefieldComponent>().End();
            _battlefieldPool = ecsService.World.GetPool<BattlefieldComponent>();
        }

        public BattleCell[] GetCells()
            => GetCells(_battlefieldFilter.GetSingle());

        public BattleCell[] GetCells(int battlefield)
        {
            ref var battlefieldComponent = ref _battlefieldPool.Get(battlefield);
            return battlefieldComponent.BattlefieldCells;
        }

        public BattleCell GetCell(int x, int y)
        {
            if (x < 0 || y < 0)
            {
                LogService.LogDebug(DebugType.Error, $"Cannot get cell {x}:{y}");
                return null;
            }

            var cells = GetCells();
            var cellIndex = y * _battlefieldConfig.BattlefieldSize + x;
            if (cellIndex >= cells.Length)
            {
                LogService.LogDebug(DebugType.Error, $"Cell {x}:{y} is out of range");
                return null;
            }

            return cells[cellIndex];
        }

        public BattleCell[] GetCellsAtY(int y)
        {
            var result = new BattleCell[_battlefieldConfig.BattlefieldSize];
            var cells = GetCells();
            var firstCellIndex = y * _battlefieldConfig.BattlefieldSize;
            for (var i = 0; i < result.Length; i++)
                result[i] = cells[firstCellIndex + i];

            return result;
        }
    }
}