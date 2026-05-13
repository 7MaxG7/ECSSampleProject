using Leopotam.EcsLite;
using UnityEngine;

namespace Utils.Extensions
{
    public static class EcsExtensions
    {
        public static int GetSingle(this EcsFilter filter)
        {
            var entitiesCount = filter.GetEntitiesCount();
            if (entitiesCount == 1)
                return filter.GetRawEntities()[0];
            
            Debug.LogError($"{entitiesCount} entities in filter: {filter}");
            return -1;
        }

        public static bool TryUnpack(this EcsPackedEntity? entityPacked, EcsWorld world, out int entity)
        {
            if (!entityPacked.HasValue)
            {
                entity = -1;
                return false;
            }

            return entityPacked.Value.Unpack(world, out entity);
        }

        public static ref TComponent GetOrAdd<TComponent>(this EcsPool<TComponent> pool, int entity) where TComponent : struct
        {
            if (pool.Has(entity))
                return ref pool.Get(entity);

            return ref pool.Add(entity);
        }
    }
}