using UnityEngine;

namespace Battle.Battlefield
{
    [CreateAssetMenu(menuName = "Configs/Battle/" + nameof(BattlefieldConfig), fileName = nameof(BattlefieldConfig), order = 0)]
    public class BattlefieldConfig : ScriptableObject
    {
        [Tooltip("Размер стороны поля боя в клетках")]
        [SerializeField] private int _battlefieldSize = 4;

        public int BattlefieldSize => _battlefieldSize;
    }
}