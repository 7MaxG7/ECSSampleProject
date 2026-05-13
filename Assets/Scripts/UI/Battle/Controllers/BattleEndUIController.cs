using Abstractions.UI.Battle;
using CustomTypes.Enums.Team;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using Infrastructure;
using Zenject;

namespace UI.Battle
{
    public class BattleEndUIController
    {
        private readonly CancellationTokenProvider _tokenProvider;
        private readonly UIConfig _uiConfig;

        private BattleEndUIView _battleEndUIView;
        private IBattleEndUIModel _battleEndUIModel;

        [Inject]
        public BattleEndUIController(CancellationTokenProvider tokenProvider, UIConfig uiConfig)
        {
            _uiConfig = uiConfig;
            _tokenProvider = tokenProvider;
        }

        public void Init(IBattleEndUIModel battleEndUIModel, BattleEndUIView battleEndUIView)
        {
            _battleEndUIModel = battleEndUIModel;
            _battleEndUIView = battleEndUIView;

            _battleEndUIView.Init(_uiConfig.DefaultAnimationDuration);
            _battleEndUIModel.Winner.Subscribe(SetWinnerLabel);

            _battleEndUIView.gameObject.SetActive(false);
        }

        public void ShowBattleEndLabel(TeamType winner)
        {
            _battleEndUIModel.Winner.Value = winner;
            ShowBattleEndPanelAsync().Forget();
        }

        private async UniTaskVoid ShowBattleEndPanelAsync()
        {
            using var localCts = _tokenProvider.CreateLocalCts();
            await _battleEndUIView.ShowAsync(localCts);
        }

        private void SetWinnerLabel(TeamType winner)
            => _battleEndUIView.SetWinnerLabel(winner == TeamType.Player
                ? Constants.WIN_END_BATTLE_LABLE
                : Constants.DEFEAT_END_BATTLE_LABLE);
    }
}