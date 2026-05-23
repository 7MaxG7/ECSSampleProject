using Abstractions;
using CustomTypes;
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

            var cts = _tokenProvider.CreateLocalCts();
            _battleEndUIModel.IsBattleEndUIVisible.Subscribe(_battleEndUIView.SetActiveAsync, cts.Token);
            _battleEndUIModel.Winner.Subscribe(SetWinnerLabel, cts.Token);

            _battleEndUIView.SetVisible(false);
        }

        private void SetWinnerLabel(TeamType winner)
            => _battleEndUIView.SetWinnerLabel(winner == TeamType.Player
                ? Constants.WIN_END_BATTLE_LABLE
                : Constants.DEFEAT_END_BATTLE_LABLE);
    }
}