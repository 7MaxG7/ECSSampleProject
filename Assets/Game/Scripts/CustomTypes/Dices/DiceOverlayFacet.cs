using System;
using CustomTypes.Enums;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace CustomTypes
{
    [Serializable]
    public class DiceOverlayFacet
    {
        [SerializeField] private DiceSideType _diceSide;
        [SerializeField] private AssetReference _overlayFacetIcon;

        public DiceSideType DiceSide => _diceSide;
        public AssetReference OverlayFacetIcon => _overlayFacetIcon;
    }
}