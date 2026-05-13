using System;
using UnityEngine;

namespace CustomTypes
{
    [Serializable]
    public struct UnitSpecialization : IEquatable<UnitSpecialization>
    {
        [SerializeField] private UnitClass _class;
        [SerializeField] private UnitArchetype _archetype;
        [SerializeField] [Min(1)] private int _level;

        public UnitSpecialization(UnitClass unitClass, UnitArchetype archetype, int level)
        {
            _class = unitClass;
            _archetype = archetype;
            _level = level;
        }

        public UnitClass Class => _class;
        public UnitArchetype Archetype => _archetype;
        public int Level => _level;

        public override string ToString()
            => $"{Class} | {Archetype} | {Level}";

        public bool Equals(UnitSpecialization other)
            => _class == other._class && _archetype == other._archetype && _level == other._level;

        public override bool Equals(object obj)
            => obj is UnitSpecialization other && Equals(other);

        public override int GetHashCode()
            => HashCode.Combine((int)Class, (int)Archetype, Level);
    }
}