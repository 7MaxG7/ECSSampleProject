using UnityEngine;

namespace Battle
{
    [CreateAssetMenu(menuName = "Configs/Battle/" + nameof(DiceRollsConfig), fileName = nameof(DiceRollsConfig), order = 0)]
    public class DiceRollsConfig : ScriptableObject
    {
        [SerializeField] private int _playerDefaultRollsCount;
        [SerializeField] private int _enemyDefaultRollsCount;

        public int PlayerDefaultRollsCount => _playerDefaultRollsCount;
        public int EnemyDefaultRollsCount => _enemyDefaultRollsCount;
    }
}