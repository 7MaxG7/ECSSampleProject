using CustomTypes;
using Leopotam.EcsLite;

namespace Units
{
    public struct UnitComponent
    {
        public string Id;
        public UnitSpecialization Specialization;
        public EcsPackedEntity Dice;
    }
}