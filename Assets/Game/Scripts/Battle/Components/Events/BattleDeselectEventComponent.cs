using Abstractions.Infrastructure;
using CustomTypes.Enums.Battle;

namespace Battle
{
    public struct BattleDeselectEventComponent : IFrameEvent
    {
        public BattleSelectionType SelectionType;
    }
}