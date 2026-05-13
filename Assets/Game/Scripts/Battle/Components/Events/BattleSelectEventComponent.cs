using Abstractions.Infrastructure;
using CustomTypes.Enums.Battle;

namespace Battle
{
    public struct BattleSelectEventComponent : IFrameEvent
    {
        public BattleSelectionType SelectionType;
    }
}