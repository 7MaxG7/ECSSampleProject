using Dices;
using UnityEngine;

namespace UI.Battle
{
    public class BattleDicesUIView : MonoBehaviour
    {
        [SerializeField] private DiceAimUIView _aimingDice;
        [SerializeField] private Transform _playerDicesContent;
        [SerializeField] private Transform _enemyDicesContent;

        public DiceAimUIView AimingDice => _aimingDice;
        public Transform PlayerDicesContent => _playerDicesContent;
        public Transform EnemyDicesContent => _enemyDicesContent;
    }
}