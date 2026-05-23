using UnityEngine;

namespace UI
{
    [CreateAssetMenu(menuName = "Configs/UI/" + nameof(UIConfig), fileName = nameof(UIConfig), order = 0)]
    public class UIConfig : ScriptableObject
    {
        [SerializeField] private DiceSideIcon[] _diceSideIcons;
        [SerializeField] private float _defaultAnimationDuration;
        [Tooltip("Продолжительность анимации изменения значения шкал здоровья")]
        [SerializeField] private float _healthBarAnimationDuration = .25f;

        public DiceSideIcon[] DiceSideIcons => _diceSideIcons;
        public float DefaultAnimationDuration => _defaultAnimationDuration;
        public float HealthBarAnimationDuration => _healthBarAnimationDuration;
    }
}