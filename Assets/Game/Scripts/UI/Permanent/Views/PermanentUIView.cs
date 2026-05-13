using UnityEngine;

namespace UI.Permanent
{
    public class PermanentUIView : MonoBehaviour
    {
        [SerializeField] private Transform _content;

        public Transform Content => _content;
    }
}