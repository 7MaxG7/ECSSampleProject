using CustomTypes;
using Leopotam.EcsLite;

namespace Dices
{
    public struct DiceComponent
    {
        public DiceSide CurrentSide;
        public EcsPackedEntity? Unit;
        public DiceConfig Config;
    }
}