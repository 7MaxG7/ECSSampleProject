using UI.Permanent;
using UnityEngine;

namespace UI
{
    [CreateAssetMenu(menuName = "Configs/UI/" + nameof(UIAssetsDb), fileName = nameof(UIAssetsDb), order = 0)]
    public class UIAssetsDb : ScriptableObject
    {
        [SerializeField] private PermanentUIView _permanentUIView;
        [SerializeField] private CurtainUIView _curtainView;

        public PermanentUIView PermanentUIView => _permanentUIView;
        public CurtainUIView CurtainView => _curtainView;
    }
}