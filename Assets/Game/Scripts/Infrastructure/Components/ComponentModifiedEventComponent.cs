using Abstractions;

namespace Infrastructure
{
    public struct ComponentModifiedEventComponent<T> : IFrameEvent where T : struct
    {
    }
}