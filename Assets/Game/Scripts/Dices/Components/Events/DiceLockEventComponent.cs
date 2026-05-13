using Abstractions.Infrastructure;

namespace Dices.Events
{
    public struct DiceLockEventComponent : IFrameEvent
    {
        public bool IsLocked;
    }
}