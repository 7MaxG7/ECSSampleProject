using System.Collections.Generic;
using Abstractions;
using Battle;
using CustomTypes;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using Dices;
using Infrastructure;
using Leopotam.EcsLite;
using UnityEngine;
using Utils;
using Zenject;

namespace UI.Battle
{
    public class BattleUIController
    {
        private readonly BattleUIFactory _battleUIFactory;
        private readonly BattleDiceRollService _rollService;

        private readonly UnitsOverlayUIController _unitsOverlayUIController;
        private readonly BattleEndUIController _battleEndUIController;
        private readonly BattleDicesUIController _battleDicesUIController;
        
        private BattleUIModel _battleUIModel;
        private BattleUIView _battleUIView;
        
        private readonly EcsFilter _mainDicesFilter;
        private readonly EcsFilter _dicesRollEventFilter;
        private readonly EcsPool<DicesRollEventComponent> _dicesRollEventPool;
        private readonly EcsPool<DiceViewComponent> _diceViewPool;

        [Inject]
        public BattleUIController(UnitsOverlayUIController unitsOverlayUIController, BattleDicesUIController battleDicesUIController,
            BattleEndUIController battleEndUIController, BattleDiceRollService rollService, BattleUIFactory battleUIFactory)
        {
            _unitsOverlayUIController = unitsOverlayUIController;
            _battleEndUIController = battleEndUIController;
            _battleDicesUIController = battleDicesUIController;

            _rollService = rollService;
            _battleUIFactory = battleUIFactory;
        }

        public async UniTask InitAsync(BattleUIModel battleUIModel, BattleUIView battleUIView)
        {
            _battleUIView = battleUIView;
            _battleUIModel = battleUIModel;

            InitRollsUI();
            await CreateUnitsOverlayUIAsync(battleUIView.RootContent);
            await CreateEndBattleUIAsync(battleUIModel, battleUIView.RootContent);
            await CreateBattleDicesUIAsync(battleUIModel, battleUIView.RootContent);
        }

        public void OnDispose()
        {
            _unitsOverlayUIController.Clear();
            _battleDicesUIController.Clear();
            _battleUIView.RerollButton.OnClick.RemoveAllListeners();
        }

        public async UniTask CreateUnitsUI(Dictionary<TeamType, List<int>> units)
        {
            foreach (var team in units.Keys)
            foreach (var unit in units[team])
                await _battleDicesUIController.AddDiceView(team, unit);

            await _unitsOverlayUIController.CreateUnitsOverlayAsync(units);
        }

        public void ToggleRollUIInteractable(bool mustInteractable)
            => _battleUIView.RerollButton.Interactable = mustInteractable;

        public void SetRollsCountLabel(TeamType team, int rollsCount)
        {
            switch (team)
            {
                case TeamType.Player:
                    _battleUIModel.PlayerRollsCount.Update((team, rollsCount));
                    break;
                case TeamType.Enemy:
                    _battleUIModel.EnemyRollsCount.Update((team, rollsCount));
                    break;
                default:
                    LogService.LogDebug(DebugType.Error, $"Cannot update rolls for team {team}");
                    return;
            }
        }

        public void UpdateRollButtonLabel(bool areDicesLocked)
            => _battleUIView.SetRollButtonLabel(areDicesLocked ? TextKeys.FINISH_ROLLING_BUTTON : TextKeys.REROLL_BUTTON);

        public void ShowCurrentDices(List<DiceData> dices, TeamType team)
            => _battleDicesUIController.ShowCurrentDices(dices, team);

        public void UpdateHealthBar(int unit)
            => _unitsOverlayUIController.UpdateHealthBar(unit);

        public void UpdateIncomingDamage(int unit, int damage)
            => _unitsOverlayUIController.UpdateIncomingDamage(unit, damage);

        public void ShowBattleEndLabel(TeamType winner)
            => _battleEndUIController.ShowBattleEndLabel(winner);

        private void InitRollsUI()
        {
            _battleUIModel.PlayerRollsCount.Subscribe(_battleUIView.SetTeamRolls);
            _battleUIModel.EnemyRollsCount.Subscribe(_battleUIView.SetTeamRolls);

            _battleUIView.RerollButton.OnClick.AddListener(_rollService.RollCurrentTeamUnlockedMainDices);

            _battleUIModel.PlayerRollsCount.Value = (TeamType.Player, 0);
            _battleUIModel.PlayerRollsCount.Value = (TeamType.Enemy, 0);
        }

        private async UniTask CreateUnitsOverlayUIAsync(Transform parent)
        {
            var unitsOverlayUIView = await _battleUIFactory.CreateUnitsOverlayUIViewAsync(parent);
            _unitsOverlayUIController.Init(unitsOverlayUIView);
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
    }
}