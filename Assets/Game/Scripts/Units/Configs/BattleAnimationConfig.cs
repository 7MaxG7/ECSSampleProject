using Sirenix.OdinInspector;
using UnityEngine;

namespace Units
{
    [CreateAssetMenu(menuName = "Configs/Battle/" + nameof(BattleAnimationConfig), fileName = nameof(BattleAnimationConfig), order = 0)]
    public class BattleAnimationConfig : ScriptableObject
    {
        [Tooltip("Задержка при переходе от окончания анимации применения кости юнита до старта применения кости второго предмета")]
        [SerializeField] private float _betweenUnitsApplyingDelay = .65f;
        
        [Header("Node names")]
        [Tooltip("Название блока анимации стойки в аниматоре")] [FoldoutGroup("Animations tech names")]
        [SerializeField] private string _idleAnimationName;
        [Tooltip("Название блока анимации смерти в аниматоре")] [FoldoutGroup("Animations tech names")]
        [SerializeField] private string _deathAnimationName;
        
        [Header("Transition names")]
        [Tooltip("Название параметра ближней атаки в аниматоре")] [FoldoutGroup("Animations tech names")]
        [SerializeField] private string _meleeAttackParameterName;
        [Tooltip("Название параметра дальней атаки в аниматоре")] [FoldoutGroup("Animations tech names")]
        [SerializeField] private string _rangeAttackParameterName;
        [Tooltip("Название параметра наложения армора в аниматоре")] [FoldoutGroup("Animations tech names")]
        [SerializeField] private string _armorCastParameterName;
        [Tooltip("Название параметра получения урона в аниматоре")] [FoldoutGroup("Animations tech names")]
        [SerializeField] private string _damageReceiveParameterName;
        [Tooltip("Название параметра смерти в аниматоре")] [FoldoutGroup("Animations tech names")]
        [SerializeField] private string _deathParameterName;

        public float BetweenUnitsApplyingDelay => _betweenUnitsApplyingDelay;
        public string IdleAnimationName => _idleAnimationName;
        public string DeathAnimationName => _deathAnimationName;
        public string MeleeAttackParameterName => _meleeAttackParameterName;
        public string RangeAttackParameterName => _rangeAttackParameterName;
        public string ArmorCastParameterName => _armorCastParameterName;
        public string DamageReceiveParameterName => _damageReceiveParameterName;
        public string DeathParameterName => _deathParameterName;
    }
}