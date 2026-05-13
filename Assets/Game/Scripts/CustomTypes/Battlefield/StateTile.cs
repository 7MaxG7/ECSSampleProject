using System;
using CustomTypes.Enums.Battle;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace CustomTypes
{
    [Serializable]
    public class StateTile
    {
        [SerializeField] private TileState _state;
        [SerializeField] private TileBase _tile;

        public TileState State => _state;
        public TileBase Tile => _tile;
    }
}