using System;
using CustomTypes;
using UnityEngine;

namespace UI
{
    [Serializable]
    public class DiceSideIcon
    {
        [SerializeField] private DiceSideType _diceSide;
        [SerializeField] private Sprite _icon;

        public DiceSideType DiceSide => _diceSide;
        public Sprite Icon => _icon;
    }
}