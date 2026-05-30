using Abstractions;
using Leopotam.EcsLite;

namespace Dices
{
    public struct DiceAppliedEventComponent : IFrameEvent
    {
        public EcsPackedEntity Target;
    }
}