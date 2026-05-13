using UnityEngine;

namespace Battle
{
    [CreateAssetMenu(menuName = "Configs/Battle/" + nameof(HighlightConfig), fileName = nameof(HighlightConfig), order = 0)]
    public class HighlightConfig : ScriptableObject
    {
        [SerializeField] private Color _defaultHighlightColor;
        [SerializeField] private Color _aimingHighlightColor;

        public Color DefaultHighlightColor => _defaultHighlightColor;
        public Color AimingHighlightColor => _aimingHighlightColor;
    }
}