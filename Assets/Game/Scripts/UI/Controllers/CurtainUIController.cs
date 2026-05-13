using System.Threading;
using Cysharp.Threading.Tasks;
using UI.Permanent;

namespace UI
{
    public class CurtainUIController
    {
        private CurtainUIView _curtainUIView;

        public void Init(CurtainUIView curtainUIView)
        {
            _curtainUIView = curtainUIView;
        }

        public void Clear()
            => _curtainUIView.Clear();

        public async UniTask Show(CancellationTokenSource cts)
            => await _curtainUIView.ToggleActiveAsync(true, cts);

        public async UniTask Hide(CancellationTokenSource cts)
            => await _curtainUIView.ToggleActiveAsync(false, cts);

        public void ShowInstantly()
            => _curtainUIView.ShowInstantly();
    }
}