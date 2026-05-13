using Abstractions;
using CustomTypes;

namespace Battle
{
    public struct BattleDeselectEventComponent : IFrameEvent
    {
        public BattleSelectionType SelectionType;
    }
}