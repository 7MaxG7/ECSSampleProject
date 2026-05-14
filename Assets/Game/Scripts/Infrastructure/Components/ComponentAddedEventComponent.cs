using Abstractions;

namespace Infrastructure
{
    public struct ComponentAddedEventComponent<T> : IFrameEvent where T : struct
    {
    }
}