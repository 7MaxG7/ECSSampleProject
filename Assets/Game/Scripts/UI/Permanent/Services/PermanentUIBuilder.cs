namespace UI.Permanent
{
    public class PermanentUIBuilder
    {
        private readonly CommonUIFactory _commonUIFactory;
        private readonly CurtainUIController _curtainUIController;
        
        private PermanentUIView _permanentUIView;

        public PermanentUIBuilder(CommonUIFactory commonUIFactory, CurtainUIController curtainUIController)
        {
            _commonUIFactory = commonUIFactory;
            _curtainUIController = curtainUIController;
        }

        public void BuildUI()
        {
            _permanentUIView = _commonUIFactory.CreateRoot();
            
            var curtainView = _commonUIFactory.CreateCurtain(_permanentUIView.Content);
            _curtainUIController.Init(curtainView);
            _curtainUIController.ShowInstantly();
        }

        public void OnDispose()
            => _curtainUIController.Clear();
    }
}