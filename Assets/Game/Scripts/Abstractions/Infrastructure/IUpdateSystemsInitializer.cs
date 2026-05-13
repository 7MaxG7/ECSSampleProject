using Leopotam.EcsLite;

namespace Abstractions.Infrastructure
{
    public interface IUpdateSystemsInitializer
    {
        void InitSystems(EcsSystems updateSystems);
    }
}