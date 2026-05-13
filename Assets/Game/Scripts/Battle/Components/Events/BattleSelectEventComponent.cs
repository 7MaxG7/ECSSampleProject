using Abstractions;
using CustomTypes;

namespace Battle
{
    public struct BattleSelectEventComponent : IFrameEvent
    {
        public BattleSelectionType SelectionType;
    }
}