using Infrastructure;

namespace UI.Permanent
{
    public class PermanentUIBuilder
    {
        private readonly CommonUIFactory _commonUIFactory;
        private readonly CurtainService _curtainService;
        private readonly CancellationTokenProvider _tokenProvider;

        private PermanentUIView _permanentUIView;
        private CurtainUIController _curtainUIController;

        public PermanentUIBuilder(CommonUIFactory commonUIFactory, CurtainService curtainService, CancellationTokenProvider tokenProvider)
        {
            _commonUIFactory = commonUIFactory;
            _curtainService = curtainService;
            _tokenProvider = tokenProvider;
        }

        public void BuildUI()
        {
            _permanentUIView = _commonUIFactory.CreateRoot();
            
            CreateCurtain();
            _curtainService.ShowInstantly();
        }

        public void OnDispose()
            => _curtainUIController.OnDispose();

        private void CreateCurtain()
        {
            var curtainModel = new CurtainUIModel();
            var curtainView = _commonUIFactory.CreateCurtain(_permanentUIView.Content);
            _curtainUIController = new CurtainUIController(_tokenProvider);
            _curtainUIController.Init(curtainModel, curtainView);
            
            _curtainService.Init(curtainModel);
        }
    }
}