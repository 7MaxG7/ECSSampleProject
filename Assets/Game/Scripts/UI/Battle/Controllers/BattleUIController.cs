using Abstractions;
using Battle;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using Infrastructure;
using Leopotam.EcsLite;
using UnityEngine;
using Zenject;

namespace UI.Battle
{
    public class BattleUIController
    {
        private readonly BattleUIFactory _battleUIFactory;
        private readonly BattleDiceRollService _rollService;
        private readonly CancellationTokenProvider _tokenProvider;

        private readonly EcsFilter _mainDicesFilter;
        private readonly EcsFilter _dicesRollEventFilter;

        private BattleUIModel _battleUIModel;
        private BattleUIView _battleUIView;
        private readonly UnitsOverlayUIController _unitsOverlayUIController;
        private readonly BattleEndUIController _battleEndUIController;
        private readonly BattleDicesUIController _battleDicesUIController;

        [Inject]
        public BattleUIController(UnitsOverlayUIController unitsOverlayUIController, BattleDicesUIController battleDicesUIController,
            BattleEndUIController battleEndUIController, BattleDiceRollService rollService, CancellationTokenProvider tokenProvider,
            BattleUIFactory battleUIFactory)
        {
            _unitsOverlayUIController = unitsOverlayUIController;
            _battleEndUIController = battleEndUIController;
            _battleDicesUIController = battleDicesUIController;

            _rollService = rollService;
            _tokenProvider = tokenProvider;
            _battleUIFactory = battleUIFactory;
        }

        public async UniTask InitAsync(BattleUIModel battleUIModel, BattleUIView battleUIView)
        {
            _battleUIView = battleUIView;
            _battleUIModel = battleUIModel;

            InitRollsUI();
            await CreateUnitsOverlayUIAsync(battleUIModel, battleUIView.RootContent);
            await CreateEndBattleUIAsync(battleUIModel, battleUIView.RootContent);
            await CreateBattleDicesUIAsync(battleUIModel, battleUIView.RootContent);
            
            SetRollUIInteractable(false);
        }

        public void OnDispose()
        {
            _unitsOverlayUIController.Clear();
            _battleDicesUIController.Clear();
            _battleUIView.RerollButton.OnClick.RemoveAllListeners();
        }

        private void InitRollsUI()
        {
            var cts = _tokenProvider.CreateLocalCts();
            foreach (var (team, rollsProperty) in _battleUIModel.DiceRolls)
                rollsProperty.Subscribe(rollsCount => _battleUIView.SetTeamRolls(team, rollsCount), cts.Token);

            _battleUIModel.IsRollUIInteractable.Subscribe(SetRollUIInteractable, cts.Token);
            _battleUIModel.AreAllTeamDicesLocked.Subscribe(
                areLocked => _battleUIView.SetRollButtonLabel(areLocked ? TextKeys.FINISH_ROLLING_BUTTON : TextKeys.REROLL_BUTTON),
                cts.Token);

            _battleUIView.RerollButton.OnClick.AddListener(_rollService.RollCurrentTeamUnlockedMainDices);
        }

        private async UniTask CreateUnitsOverlayUIAsync(IUnitsOverlayUIModel unitsOverlayUIModel, Transform parent)
        {
            var unitsOverlayUIView = await _battleUIFactory.CreateUnitsOverlayUIViewAsync(parent);
            _unitsOverlayUIController.Init(unitsOverlayUIModel, unitsOverlayUIView);
        }

        private async UniTask CreateEndBattleUIAsync(IBattleEndUIModel battleEndUIModel, Transform parent)
        {
            var endBattleUIView = await _battleUIFactory.CreateEndBattleUIViewAsync(parent);
            _battleEndUIController.Init(battleEndUIModel, endBattleUIView);
        }

        private async UniTask CreateBattleDicesUIAsync(IBattleDicesUIModel dicesUIModel, Transform parent)
        {
            var battleDicesUIView = await _battleUIFactory.CreateBattleDicesUIViewAsync(parent);
            _battleDicesUIController.Init(dicesUIModel, battleDicesUIView);
        }

        private void SetRollUIInteractable(bool mustInteractable)
            => _battleUIView.RerollButton.Interactable = mustInteractable;
    }
}