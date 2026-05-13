using CustomTypes;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Battle.Battlefield
{
    [CreateAssetMenu(menuName = "Configs/Battle/" + nameof(BattlefieldViewConfig), fileName = nameof(BattlefieldViewConfig), order = 0)]
    public class BattlefieldViewConfig : ScriptableObject
    {
        [SerializeField] private AssetReference _battlefieldPref;
        [SerializeField] private StateTile[] _stateTiles;

        public AssetReference BattlefieldPref => _battlefieldPref;
        public StateTile[] StateTiles => _stateTiles;
    }
}