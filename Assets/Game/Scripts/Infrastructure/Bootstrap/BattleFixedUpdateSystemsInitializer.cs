using Abstractions.Infrastructure;
using Leopotam.EcsLite;

namespace Infrastructure.Bootstrap
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