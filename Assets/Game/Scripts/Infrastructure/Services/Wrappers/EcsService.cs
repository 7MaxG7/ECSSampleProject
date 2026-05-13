using System.Collections.Generic;
using System.Linq;
using CustomTypes;
using Leopotam.EcsLite;
using Leopotam.EcsLite.UnityEditor;
using UnityEngine;
using Utils;
using Zenject;

namespace Infrastructure
{
    public class EcsService
    {
        public EcsWorld World { get; private set; }
        
        private readonly SceneLoader _sceneLoader;
        private readonly EcsWorldDebugSystem _ecsWorldDebugSystem;

        private readonly Dictionary<string, EcsSystems> _systems = new();

        [Inject]
        public EcsService(SceneLoader sceneLoader, EcsWorldDebugSystem ecsWorldDebugSystem)
        {
            _sceneLoader = sceneLoader;
            _ecsWorldDebugSystem = ecsWorldDebugSystem;
        }

        public void Init()
        {
            World = new EcsWorld();
        }

        public EcsSystems CreateSystems(bool isFixed, bool isInfrastructure = false)
        {
            var key = isInfrastructure ? Constants.INFRASTRUCTURE_SYSTEMS_KEY : _sceneLoader.GetCurrentSceneName();
            if (isFixed)
                key += Constants.FIXED_SYSTEMS_KEY_POSTFIX;
            
            var systems = new EcsSystems(World);
            _systems[key] = systems;
            return systems;
        }

        public int CreateEntity()
            => World.NewEntity();

        public void AddEntityDebugView(GameObject gObj, int entity)
        {
#if UNITY_EDITOR
            var ecsEntityDebugView = gObj.AddComponent<EcsEntityDebugView>();
            ecsEntityDebugView.Entity = entity;
            ecsEntityDebugView.World = World;
            ecsEntityDebugView.DebugSystem = _ecsWorldDebugSystem;
#endif

        }

        public void DestroyEntity(int entity)
            => World.DelEntity(entity);

        public void DestroySceneSystems()
        {
            // Systems can be cleared by DestroyAll on game closing
            if (!IsInited())
                return;
            
            DestroySystems(false);
            DestroySystems(true);
        }

        public void DestroyAll()
        {
            foreach (var system in _systems.Values.ToArray())
                system?.Destroy();
            _systems.Clear();
            
            World?.Destroy();
            World = null;
        }

        public bool IsInited()
        {
            return World != null;
        }

        public bool TryUnpack(EcsPackedEntity? entityPacked, out int entity)
            => entityPacked.TryUnpack(World, out entity);

        public bool TryUnpackWithWarning(EcsPackedEntity? entityPacked, out int entity)
        {
            if (entityPacked.TryUnpack(World, out entity))
                return true;
            
            LogService.LogDebug(DebugType.Warning, "Cannot unpack entity");
            return false;
        }

        public bool TryUnpackWithWarning(EcsPackedEntity entityPacked, out int entity)
        {
            if (entityPacked.Unpack(World, out entity))
                return true;
            
            LogService.LogDebug(DebugType.Warning, "Cannot unpack entity");
            return false;
        }

        private void DestroySystems(bool isFixed)
        {
            var key = _sceneLoader.GetCurrentSceneName();
            if (isFixed)
                key += Constants.FIXED_SYSTEMS_KEY_POSTFIX;
            
            var systems = _systems[key];
            systems?.Destroy();
            _systems.Remove(key);
        }
    }
}