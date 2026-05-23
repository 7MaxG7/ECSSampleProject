using System.Threading;
using Cysharp.Threading.Tasks;
using Infrastructure;

namespace UI.Permanent
{
    public class CurtainUIController
    {
        private readonly CancellationTokenProvider _tokenProvider;
        private CurtainUIModel _curtainModel;
        private CurtainUIView _curtainView;

        public CurtainUIController(CancellationTokenProvider tokenProvider)
        {
            _tokenProvider = tokenProvider;
        }

        public void Init(CurtainUIModel curtainModel, CurtainUIView curtainView)
        {
            _curtainModel = curtainModel;
            _curtainView = curtainView;
            
            _curtainModel.IsActive.SubscribeUts(SetActiveAsync, _tokenProvider.CreateLocalCts().Token);
        }

        public void OnDispose()
        {
            _curtainView.Clear();
        }

        private async UniTask SetActiveAsync(bool isActive, CancellationToken token)
        {
            if (_curtainModel.IsInstantAnimation)
                _curtainView.SetActiveInstantly(isActive);
            else
                await _curtainView.SetActiveAsync(isActive, token);
        }
    }
}