using System.Collections.Generic;
using Abstractions.UI.Battle;
using Battle;
using CustomTypes;
using CustomTypes.Enums.Infrastructure;
using CustomTypes.Enums.Team;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using Dices;
using Dices.Events;
using Infrastructure;
using Leopotam.EcsLite;
using UnityEngine;
using Zenject;

namespace UI.Battle
{
    public class BattleUIController
    {
        private BattleUIFactory _battleUIFactory;
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
            BattleEndUIController battleEndUIController, BattleDiceRollService rollService)
        {
            _unitsOverlayUIController = unitsOverlayUIController;
            _battleEndUIController = battleEndUIController;
            _battleDicesUIController = battleDicesUIController;

            _rollService = rollService;
        }

        public async UniTask InitAsync(BattleUIModel battleUIModel, BattleUIView battleUIView, BattleUIFactory battleUIFactory)
        {
            _battleUIFactory = battleUIFactory;
            _battleUIView = battleUIView;
            _battleUIModel = battleUIModel;

            InitRollsUI();
            await CreateUnitsOverlayUIAsync(battleUIView.RootContent);
            await CreateEndBattleUIAsync(battleUIModel, battleUIView.RootContent);
            await CreateBattleDicesUIAsync(battleUIModel, battleUIView.RootContent);
        }

        public void Clear()
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

        public void UpdateRollsCountLabel(TeamType team, int rollsCount)
        {
            switch (team)
            {
                case TeamType.Player:
                    _battleUIModel.PlayerRollsCount.Value = (team, rollsCount);
                    break;
                case TeamType.Enemy:
                    _battleUIModel.EnemyRollsCount.Value = (team, rollsCount);
                    break;
                default:
                    LogService.LogDebug(DebugType.Error, $"Cannot update rolls for team {team}");
                    return;
            }
        }

        public void UpdateRollButtonLabel(bool areDicesLocked)
            => _battleUIView.RerollButton.Text = areDicesLocked ? TextKeys.FINISH_ROLLING_BUTTON : TextKeys.REROLL_BUTTON;

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
            _battleUIModel.PlayerRollsCount.Subscribe(_battleUIView.UpdateTeamRolls);
            _battleUIModel.EnemyRollsCount.Subscribe(_battleUIView.UpdateTeamRolls);

            _battleUIView.RerollButton.OnClick.AddListener(_rollService.RollCurrentTeamUnlockedMainDices);

            UpdateRollsCountLabel(TeamType.Player, 0);
            UpdateRollsCountLabel(TeamType.Enemy, 0);
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