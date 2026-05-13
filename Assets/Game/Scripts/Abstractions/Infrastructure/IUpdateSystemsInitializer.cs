using Leopotam.EcsLite;

namespace Abstractions
{
    public interface IUpdateSystemsInitializer
    {
        void InitSystems(EcsSystems updateSystems);
    }
}