using Abstractions;
using Leopotam.EcsLite;

namespace Infrastructure
{
    public class BattleFixedUpdateSystemsInitializer : IUpdateSystemsInitializer
    {
        public void InitSystems(EcsSystems fixedUpdateSystems)
        {
            fixedUpdateSystems.
                Init();
        }
    }
}