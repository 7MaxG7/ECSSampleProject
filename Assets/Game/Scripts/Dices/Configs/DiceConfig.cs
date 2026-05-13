using System;
using CustomTypes;
using UnityEngine;

namespace Dices
{
    [Serializable]
    public class DiceConfig
    {
        [SerializeField] private DiceSide[] _sides;

        public DiceSide[] Sides => _sides;
    }
}