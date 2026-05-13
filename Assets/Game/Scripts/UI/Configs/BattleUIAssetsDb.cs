using System.Linq;
using CustomTypes;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace UI
{
    [CreateAssetMenu(menuName = "Configs/UI/" + nameof(BattleUIAssetsDb), fileName = nameof(BattleUIAssetsDb), order = 0)]
    public class BattleUIAssetsDb : ScriptableObject
    {
        [Header("Battle")]
        [SerializeField] private AssetReference _battleUIView;
        [SerializeField] private AssetReference _battleEndUIView;
        [SerializeField] private AssetReference _battleDicesUIView;
        [SerializeField] private AssetReference _diceUIView;
        [SerializeField] private AssetReference _diceAimUIView;
        [Header("Overlay")]
        [Tooltip("ЮИ для оверлеев юнитов")]
        [SerializeField] private AssetReference _battleUnitsOverlayUIView;
        [Tooltip("Оверлей юнита")]
        [SerializeField] private AssetReference _unitUIOverlayView;
        [SerializeField] private DiceOverlayFacet[] _diceOverlayFacets;

        public AssetReference BattleUIView => _battleUIView;
        public AssetReference BattleEndUIView => _battleEndUIView;
        public AssetReference DiceUIView => _diceUIView;
        public AssetReference DiceAimUIView => _diceAimUIView;
        public AssetReference BattleUnitsOverlayUIView => _battleUnitsOverlayUIView;
        public AssetReference UnitUIOverlayView => _unitUIOverlayView;

        public AssetReference BattleDicesUIView => _battleDicesUIView;

        public AssetReference GetOverlayFacetIcon(DiceSide diceSide)
            => _diceOverlayFacets.First(item => item.DiceSide == diceSide.SideType).OverlayFacetIcon;
    }
}