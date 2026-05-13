using Battle;
using CustomTypes;
using UnityEngine;

namespace Units
{
    public class UnitView : MonoBehaviour
    {
        [SerializeField] private Renderer _renderer;
        [SerializeField] private HighlightView _highlight;
        [SerializeField] private BattleSelectView _selectView;
        [SerializeField] private Transform _uiOverlayAnchor;
        [SerializeField] private Animator _animator;

        public HighlightView Highlight => _highlight;
        public BattleSelectView SelectView => _selectView;
        public Transform UIOverlayAnchor => _uiOverlayAnchor;
        public Animator Animator => _animator;

        public void SetTeam(TeamType team)
            => _renderer.material.color = team == TeamType.Player ? Color.cyan : Color.red;
    }
}