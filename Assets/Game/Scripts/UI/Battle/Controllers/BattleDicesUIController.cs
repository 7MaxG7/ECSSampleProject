using System.Collections.Generic;
using System.Threading;
using Abstractions;
using Battle;
using CustomTypes;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using Dices;
using Infrastructure;
using Units;
using Zenject;

namespace UI.Battle
{
    public class BattleDicesUIController
    {
        private readonly UnitService _unitService;
        private readonly BattleUIFactory _battleUIFactory;
        private readonly DiceTargetSelectService _diceTargetSelectService;
        private readonly BattleDiceLockService _diceLockService;
        private readonly HighlightService _highlightService;
        private readonly DiceAimingService _diceAimingService;
        private readonly CancellationTokenProvider _tokenProvider;

        private BattleDicesUIView _battleDicesUIView;
        private IBattleDicesUIModel _battleDicesUIModel;

        private readonly Dictionary<string, DiceUIView> _diceViews = new();

        [Inject]
        public BattleDicesUIController(EcsService ecsService, BattleUIFactory battleUIFactory, BattleDiceLockService diceLockService,
            DiceTargetSelectService diceTargetSelectService, HighlightService highlightService,
            DiceAimingService diceAimingService, CancellationTokenProvider tokenProvider, UnitService unitService)
        {
            _unitService = unitService;
            _battleUIFactory = battleUIFactory;
            _diceTargetSelectService = diceTargetSelectService;
            _diceLockService = diceLockService;
            _highlightService = highlightService;
            _diceAimingService = diceAimingService;
            _tokenProvider = tokenProvider;
        }

        public void Init(IBattleDicesUIModel battleDicesUIModel, BattleDicesUIView battleDicesUIView)
        {
            _battleDicesUIModel = battleDicesUIModel;
            _battleDicesUIView = battleDicesUIView;

            _battleDicesUIModel.AreDicesAdded.Subscribe(AddNewDices, _tokenProvider.CreateLocalCts().Token);
        }

        public void Clear()
        {
            foreach (var diceView in _diceViews.Values)
                UnsubscribeDiceView(diceView);
        }

        private async UniTaskVoid AddNewDices(CancellationToken token)
        {
            foreach (var (team, dices) in _battleDicesUIModel.DiceModels)
            foreach (var (id, diceModel) in dices)
            {
                if (_diceViews.ContainsKey(id))
                    continue;

                if (!_unitService.TryGetUnit(id, out var unit) || !_unitService.TryGetDice(unit, out var dice))
                {
                    LogService.LogDebug(DebugType.Error, $"Cannot find unit with id {id} or dice");
                    continue;
                }

                var content = team switch
                {
                    TeamType.Player => _battleDicesUIView.PlayerDicesContent,
                    TeamType.Enemy => _battleDicesUIView.EnemyDicesContent,
                    _ => null,
                };

                var diceView = await _battleUIFactory.CreateDiceUIViewAsync(dice, content);
                diceModel.IsVisible.Subscribe(diceView.SetVisible, token);
                diceModel.IsInteractable.Subscribe(diceView.SetInteractable, token);
                diceModel.IsLocked.Subscribe(diceView.SetLocked, token);
                diceModel.DiceSide.Subscribe(diceView.SetCurrentSide, token);
                diceModel.IsDimmed.Subscribe(diceView.SetDimmed, token);
                diceModel.IsLit.Subscribe(diceView.Highlight.SetHighlightEnabled, token);

                SubscribeDiceView(diceView, dice);
                _diceViews[id] = diceView;
            }
        }

        private void SubscribeDiceView(DiceUIView diceView, int dice)
        {
            diceView.LockButton.OnClick.AddListener(() => _diceLockService.ToggleDiceLock(dice));
            diceView.OnDicePointed += EnableDiceUnitHighlight;
            diceView.OnDiceUnpointed += DisableDiceUnitHighlight;
            diceView.OnDiceDragBegin += StartDiceAiming;
            diceView.OnDiceDragEnd += TrySetTarget;
        }

        private void UnsubscribeDiceView(DiceUIView diceView)
        {
            diceView.LockButton.OnClick.RemoveAllListeners();
            diceView.OnDicePointed -= EnableDiceUnitHighlight;
            diceView.OnDiceUnpointed -= DisableDiceUnitHighlight;
            diceView.OnDiceDragBegin -= StartDiceAiming;
            diceView.OnDiceDragEnd -= TrySetTarget;
        }

        private void EnableDiceUnitHighlight(DiceUIView diceUIView)
            => SetDiceUnitHighlight(diceUIView, true);

        private void DisableDiceUnitHighlight(DiceUIView diceUIView)
            => SetDiceUnitHighlight(diceUIView, false);

        private void StartDiceAiming(DiceUIView diceUIView)
        {
            if (!_diceTargetSelectService.IsTargetSelecting || !TryGetDiceOwner(diceUIView, out var owner) ||
                !_unitService.TryGetDice(owner, out var dice))
                return;
            
            _diceAimingService.StartDiceAiming(dice);
        }

        private void TrySetTarget(DiceUIView diceUIView)
        {
            if (TryGetDiceOwner(diceUIView, out var owner) && _unitService.TryGetDice(owner, out var dice))
                _diceTargetSelectService.TrySetTarget(dice);
        }

        private void SetDiceUnitHighlight(DiceUIView diceUIView, bool mustLit)
        {
            diceUIView.Highlight.SetHighlightEnabled(mustLit);
            if (TryGetDiceOwner(diceUIView, out var unit))
                _highlightService.SetHighlight(unit, mustLit);
        }

        private bool TryGetDiceOwner(DiceUIView diceUIView, out int owner)
        {
            foreach (var (id, diceView) in _diceViews)
            {
                if (diceView != diceUIView)
                    continue;

                return _unitService.TryGetUnit(id, out owner);
            }

            owner = -1;
            return false;
        }
    }
}