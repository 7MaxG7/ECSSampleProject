using Cysharp.Threading.Tasks;
using Infrastructure;
using Zenject;

namespace UI.Battle
{
    public class BattleUIBuilder
    {
        private readonly BattleUIModel _battleUIModel;
        private readonly BattleUIFactory _battleUIFactory;
        private readonly BattleUIController _battleUIController;

        [Inject]
        public BattleUIBuilder(EcsService ecsService, BattleUIModel battleUIModel, BattleUIController battleUIController,
            BattleUIFactory battleUIFactory)
        {
            _battleUIModel = battleUIModel;
            _battleUIFactory = battleUIFactory;
            _battleUIController = battleUIController;
        }

        public async UniTask BuildBattleUIAsync()
        {
            var battleUIView = await _battleUIFactory.CreateBattleUIViewAsync();
            await _battleUIController.InitAsync(_battleUIModel, battleUIView);
        }

        public void OnDispose()
        {
            _battleUIController.OnDispose();
        }
    }
}