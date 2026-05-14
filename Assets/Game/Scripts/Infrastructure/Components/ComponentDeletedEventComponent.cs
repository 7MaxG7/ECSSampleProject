using Abstractions;

namespace Infrastructure
{
    public struct ComponentDeletedEventComponent<T> : IFrameEvent where T : struct
    {
    }
}