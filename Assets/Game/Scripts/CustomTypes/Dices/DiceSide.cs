using System;
using CustomTypes.Enums;
using Sirenix.OdinInspector;
using UnityEngine;

namespace CustomTypes
{
    [Serializable]
    public class DiceSide
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
                result += $":{_value}";
            
            return result;
        }
    }
}