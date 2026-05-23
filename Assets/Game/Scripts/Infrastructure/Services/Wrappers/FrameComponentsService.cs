using System;
using System.Collections.Generic;
using Leopotam.EcsLite;
using Zenject;

namespace Infrastructure
{
    public class FrameComponentsService
    {
        private const bool ARE_ONLY_CURRENT_FRAME_COMPONENTS_CLEARED = true;

        private readonly EcsService _ecsService;

        private readonly Dictionary<Type, (EcsFilter Filter, Action<int> DelAction)> _frameFiltersCache = new();
        private readonly HashSet<Type> _addedFrameComponents = new();

        [Inject]
        public FrameComponentsService(EcsService ecsService)
        {
            _ecsService = ecsService;
        }

        public bool TryAddAddedEvent<TComponent>(int entity) where TComponent : struct
            => TryAddEvent<ComponentAddedEventComponent<TComponent>>(entity);

        public bool TryAddDeletedEvent<TComponent>(int entity) where TComponent : struct
            => TryAddEvent<ComponentDeletedEventComponent<TComponent>>(entity);

        public bool TryAddModifiedEvent<TComponent>(int entity) where TComponent : struct
            => TryAddEvent<ComponentModifiedEventComponent<TComponent>>(entity);

        public void AddAddedEvent<TComponent>(int entity) where TComponent : struct
            => AddEvent<ComponentAddedEventComponent<TComponent>>(entity);

        public void AddDeletedEvent<TComponent>(int entity) where TComponent : struct
            => AddEvent<ComponentDeletedEventComponent<TComponent>>(entity);

        public void AddModifiedEvent<TComponent>(int entity) where TComponent : struct
            => AddEvent<ComponentModifiedEventComponent<TComponent>>(entity);

        public void DelAddedEvent<TComponent>(int entity) where TComponent : struct
            => DelEvent<ComponentAddedEventComponent<TComponent>>(entity);

        public void DelDeletedEvent<TComponent>(int entity) where TComponent : struct
            => DelEvent<ComponentDeletedEventComponent<TComponent>>(entity);

        public bool HasAddedEvent<TComponent>(int entity) where TComponent : struct
            => HasEvent<ComponentAddedEventComponent<TComponent>>(entity);

        public bool HasModifiedEvent<TComponent>(int entity) where TComponent : struct
            => HasEvent<ComponentModifiedEventComponent<TComponent>>(entity);

        public bool TryAddEvent<TComponent>(int entity) where TComponent : struct
        {
            if (HasEvent<TComponent>(entity))
                return false;

            AddEvent<TComponent>(entity);
            return true;
        }

        public ref TComponent AddEvent<TComponent>(int entity) where TComponent : struct
        {
            ref var component = ref _ecsService.World.GetPool<TComponent>().Add(entity);
            CacheFrameComponent<TComponent>();
            return ref component;
        }

        public ref TComponent GetOrAddEvent<TComponent>(int entity) where TComponent : struct
        {
            if (HasEvent<TComponent>(entity))
                return ref GetEvent<TComponent>(entity);

            return ref AddEvent<TComponent>(entity);
        }

        public bool HasEvent<TComponent>(int entity) where TComponent : struct
            => _ecsService.World.GetPool<TComponent>().Has(entity);

        public ref TComponent GetEvent<TComponent>(int entity) where TComponent : struct
            => ref _ecsService.World.GetPool<TComponent>().Get(entity);

        public void ClearFrameComponents()
        {
            if (ARE_ONLY_CURRENT_FRAME_COMPONENTS_CLEARED)
            {
                foreach (var component in _addedFrameComponents)
                foreach (var entity in _frameFiltersCache[component].Filter)
                    _frameFiltersCache[component].DelAction(entity);
                _addedFrameComponents.Clear();
            }
            // else
            // {
            //     foreach (var (_, (filter, delAction)) in _frameFiltersCache)
            //     foreach (var entity in filter)
            //         delAction(entity);
            // }
        }

        public EcsWorld.Mask GetAddedEventMask<TComponent>() where TComponent : struct
            => GetEventMask<ComponentAddedEventComponent<TComponent>>();

        public EcsFilter GetAddedEventFilter<TComponent>() where TComponent : struct
            => GetAddedEventMask<TComponent>().End();

        public EcsWorld.Mask GetDeletedEventMask<TComponent>() where TComponent : struct
            => GetEventMask<ComponentDeletedEventComponent<TComponent>>();

        public EcsFilter GetDeletedEventFilter<TComponent>() where TComponent : struct
            => GetDeletedEventMask<TComponent>().End();

        public EcsWorld.Mask GetModifiedEventMask<TComponent>() where TComponent : struct
            => GetEventMask<ComponentModifiedEventComponent<TComponent>>();

        public EcsFilter GetModifiedEventFilter<TComponent>() where TComponent : struct
            => GetModifiedEventMask<TComponent>().End();

        public EcsWorld.Mask GetEventMask<TComponent>() where TComponent : struct
            => _ecsService.World.Filter<TComponent>();

        public EcsFilter GetEventFilter<TComponent>() where TComponent : struct
            => GetEventMask<TComponent>().End();

        private void DelEvent<TComponent>(int entity) where TComponent : struct
            => _ecsService.World.GetPool<TComponent>().Del(entity);

        private void CacheFrameComponent<TComponent>() where TComponent : struct
        {
            var type = typeof(TComponent);
            if (ARE_ONLY_CURRENT_FRAME_COMPONENTS_CLEARED)
                _addedFrameComponents.Add(type);

            _frameFiltersCache.TryAdd(type,
                (_ecsService.World.Filter<TComponent>().End(), entity => _ecsService.World.GetPool<TComponent>().Del(entity)));
        }
    }
}