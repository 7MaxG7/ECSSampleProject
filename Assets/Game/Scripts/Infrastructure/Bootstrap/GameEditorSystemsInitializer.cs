using Leopotam.EcsLite;
using Leopotam.EcsLite.UnityEditor;
using Zenject;

namespace Infrastructure.Bootstrap
{
    public class GameEditorSystemsInitializer
    {
        private readonly EcsWorldDebugSystem _ecsWorldDebugSystem;

        [Inject]
        public GameEditorSystemsInitializer(EcsWorldDebugSystem ecsWorldDebugSystem)
        {
            _ecsWorldDebugSystem = ecsWorldDebugSystem;
        }

        public void InitSystems(EcsSystems editorSystems)
        {
            editorSystems.
#if UNITY_EDITOR
                Add (_ecsWorldDebugSystem).
#endif
                Init();
        }
    }
}