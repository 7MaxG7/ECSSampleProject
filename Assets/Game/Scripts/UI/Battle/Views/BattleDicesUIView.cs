using UnityEngine;

namespace UI.Battle
{
    public class BattleDicesUIView : MonoBehaviour
    {
        [SerializeField] private Transform _playerDicesContent;
        [SerializeField] private Transform _enemyDicesContent;
        
        public Transform PlayerDicesContent => _playerDicesContent;
        public Transform EnemyDicesContent => _enemyDicesContent;
    }
}