using UnityEngine;

namespace UI.Battle
{
    public class BattleUnitsOverlayUIView : MonoBehaviour
    {
        [SerializeField] private Transform _overlaysContent;

        public Transform OverlaysContent => _overlaysContent;
    }
}