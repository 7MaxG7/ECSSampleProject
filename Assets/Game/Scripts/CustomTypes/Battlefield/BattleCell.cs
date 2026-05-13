using Leopotam.EcsLite;
using UnityEngine;

namespace CustomTypes
{
    public class BattleCell
    {
        public Vector3Int Location { get; }
        public EcsPackedEntity? Occupier;
        
        public BattleCell(int x, int y)
        {
            Location = new Vector3Int(x, y, 0);
        }

        public override string ToString()
            => $"({Location.x} , {Location.y})";
    }
}