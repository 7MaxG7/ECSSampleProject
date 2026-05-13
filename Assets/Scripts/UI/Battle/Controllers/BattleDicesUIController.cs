using System.Collections.Generic;
using Abstractions.UI.Battle;
using Battle;
using CustomTypes;
using CustomTypes.Enums.Infrastructure;
using CustomTypes.Enums.Team;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using Dices;
using Infrastructure;
using Leopotam.EcsLite;
using Units;
using Zenject;

namespace UI.Battle
{
    public class BattleDicesUIController
    {
        private readonly EcsService _ecsService;
        private readonly UnitService _unitService;
        private readonly BattleUIFactory _battleUIFactory;
        private readonly BattleDiceService _battleDiceService;
        private readonly DiceTargetSelectService _diceTargetSelectService;
        private readonly BattleDiceLockService _diceLockService;
        private readonly HighlightService _highlightService;
        private readonly DiceAimingService _diceAimingService;

        private readonly EcsFilter _mainDicesFilter;
        private readonly EcsPool<DiceViewComponent> _diceViewPool;

        private BattleDicesUIView _battleDicesUIView;
        private IBattleDicesUIModel _dicesUIModel;

        [Inject]
        public BattleDicesUIController(EcsService ecsService, BattleUIFactory battleUIFactory, BattleDiceLockService diceLockService,
            BattleDiceService battleDiceService, DiceTargetSelectService diceTargetSelectService, HighlightService highlightService,
            UnitService unitService, DiceAimingService diceAimingService)
        {
            _ecsService = ecsService;
            _unitService = unitService;
            _battleUIFactory = battleUIFactory;
            _battleDiceService = battleDiceService;
            _diceTargetSelectService = diceTargetSelectService;
            _diceLockService = diceLockService;
            _highlightService = highlightService;
            _diceAimingService = diceAimingService;

            _mainDicesFilter = ecsService.World.Filter<DiceComponent>().End();
            _diceViewPool = ecsService.World.GetPool<DiceViewComponent>();
        }

        public void Init(IBattleDicesUIModel dicesUIModel, BattleDicesUIView battleDicesUIView)
        {
            _dicesUIModel = dicesUIModel;
            _battleDicesUIView = battleDicesUIView;
            dicesUIModel.PlayerMainDices.Subscribe(battleDicesUIView.ShowTeamDices);
            dicesUIModel.EnemyMainDices.Subscribe(battleDicesUIView.ShowTeamDices);
        }

        public void Clear()
        {
            foreach (var dice in _mainDicesFilter)
                UnsubscribeDiceView(dice);
        }

        public async UniTask AddDiceView(TeamType team, int unit)
        {
            if (!_unitService.TryGetMainDice(unit, out var dice))
                return;

            var content = team switch
            {
                TeamType.Player => _battleDicesUIView.PlayerDicesContent,
                TeamType.Enemy => _battleDicesUIView.EnemyDicesContent,
                _ => null,
            };

            var diceView = await _battleUIFactory.CreateDiceUIViewAsync(dice, content);
            SubscribeDiceView(diceView, dice);

            diceView.LockButton.Interactable = false;
            diceView.SetCurrentSide(_battleDiceService.GetCurrentSide(dice));
            _battleDicesUIView.AddDiceUI(team, unit, diceView);
        }

        public void ShowCurrentDices(List<DiceData> dices, TeamType team)
        {
            switch (team)
            {
                case TeamType.Player:
                    _dicesUIModel.PlayerMainDices.Value = (team, dices);
                    break;
                case TeamType.Enemy:
                    _dicesUIModel.EnemyMainDices.Value = (team, dices);
                    break;
                default:
                    LogService.LogDebug(DebugType.Error, $"Cannot show dices for team {team}");
                    return;
            }
        }

        private void SubscribeDiceView(DiceUIView diceView, int dice)
        {
            diceView.LockButton.OnClick.AddListener(() => _diceLockService.ToggleDiceLock(dice));
            diceView.OnDicePointed += EnableDiceUnitHighlight;
            diceView.OnDiceUnpointed += DisableDiceUnitHighlight;
            diceView.OnDiceDragBegin += StartDiceAiming;
            diceView.OnDiceDragEnd += _diceTargetSelectService.TrySetTarget;
        }

        private void UnsubscribeDiceView(int dice)
        {
            ref var diceViewComponent = ref _diceViewPool.Get(dice);
            var diceView = diceViewComponent.DiceView;

            diceView.LockButton.OnClick.RemoveAllListeners();
            diceView.OnDicePointed -= EnableDiceUnitHighlight;
            diceView.OnDiceUnpointed -= DisableDiceUnitHighlight;
            diceView.OnDiceDragBegin -= StartDiceAiming;
            diceView.OnDiceDragEnd -= _diceTargetSelectService.TrySetTarget;

            _diceViewPool.Del(dice);
        }

        private void EnableDiceUnitHighlight(EcsPackedEntity? dicePacked)
            => SetDiceUnitHighlight(dicePacked, true);

        private void DisableDiceUnitHighlight(EcsPackedEntity? dicePacked)
            => SetDiceUnitHighlight(dicePacked, false);

        private void StartDiceAiming(EcsPackedEntity? dicePacked)
        {
            if (!_diceTargetSelectService.IsTargetSelecting)
                return;

            _diceAimingService.StartDiceAiming(dicePacked);
        }

        private void SetDiceUnitHighlight(EcsPackedEntity? dicePacked, bool mustHighlighted)
        {
            if (!_ecsService.TryUnpackWithWarning(dicePacked, out var dice) || !_battleDiceService.TryGetUnit(dice, out var unit))
                return;

            _highlightService.SetHighlight(unit, mustHighlighted);
        }
    }
}