using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace CustomTypes
{
    [Serializable]
    public class DiceSide : IEquatable<DiceSide>
    {
        [SerializeField] private DiceSideType _sideType;
        [SerializeField] [ShowIf(nameof(IsValued))] private int _value;

        public DiceSideType SideType => _sideType;
        public int Value => _value;

        public bool IsValued => _sideType is DiceSideType.Armor or DiceSideType.MeleeAttack or DiceSideType.RangeAttack;

        public override string ToString()
        {
            var result = $"{_sideType}";
            if (IsValued)
                result += $":\n{_value}";
            
            return result;
        }

        public bool Equals(DiceSide other)
            => other is not null && (ReferenceEquals(this, other) || _sideType == other._sideType && _value == other._value);

        public override bool Equals(object obj)
            => obj is DiceSide side && Equals(side);

        public override int GetHashCode()
            => HashCode.Combine((int)_sideType, _value);
    }
}