using Battle;
using CustomTypes;
using Cysharp.Threading.Tasks;
using Dices;
using Infrastructure;
using Leopotam.EcsLite;
using UI.Units;
using UnityEngine;
using Zenject;

namespace UI.Battle
{
    public class BattleUIFactory
    {
        private readonly BattleUIAssetsDb _battleUIAssetsDb;
        private readonly BattleDiceLockService _diceLockService;
        private readonly AssetsProvider _assetsProvider;
        private readonly EcsService _ecsService;
        private readonly HighlightService _highlightService;

        private readonly EcsPool<DiceViewComponent> _diceViewPool;

        private Transform _rootCanvas;
        private Transform _overlayCanvas;

        [Inject]
        public BattleUIFactory(EcsService ecsService, HighlightService highlightService, BattleDiceLockService diceLockService,
            AssetsProvider assetsProvider, BattleUIAssetsDb battleUIAssetsDb)
        {
            _battleUIAssetsDb = battleUIAssetsDb;
            _diceLockService = diceLockService;
            _assetsProvider = assetsProvider;
            _ecsService = ecsService;
            _highlightService = highlightService;

            _diceViewPool = ecsService.World.GetPool<DiceViewComponent>();
        }

        public async UniTask<BattleUIView> CreateBattleUIViewAsync()
        {
            var battleUIView = await _assetsProvider.CreateInstanceAsync<BattleUIView>(_battleUIAssetsDb.BattleUIView);
            if (_rootCanvas == null)
                _rootCanvas = battleUIView.RootContent;

            return battleUIView;
        }

        public async UniTask<DiceUIView> CreateDiceUIViewAsync(int dice, Transform parent)
        {
            var diceUIView = await _assetsProvider.CreateInstanceAsync<DiceUIView>(_battleUIAssetsDb.DiceUIView, parent);

            _diceViewPool.Add(dice) = new DiceViewComponent
            {
                DiceView = diceUIView,
            };
            _ecsService.AddEntityDebugView(diceUIView.gameObject, dice);
            _highlightService.InitComponents(dice, diceUIView.Highlight);

            diceUIView.SetLocked(_diceLockService.IsLocked(dice));
            diceUIView.SetHidden(false);
            return diceUIView;
        }

        public async UniTask<DiceAimUIView> CreateDiceAimViewAsync(DiceSide diceSide, Vector3 position)
        {
            var diceUIView =
                await _assetsProvider.CreateInstanceAsync<DiceAimUIView>(_battleUIAssetsDb.DiceAimUIView, position, Quaternion.identity,
                    _rootCanvas);
            diceUIView.SetSide(diceSide);
            return diceUIView;
        }

        public async UniTask<UnitOverlayUIView> CreateUnitOverlayViewAsync(Vector3 position, Transform parent)
            => await _assetsProvider.CreateInstanceAsync<UnitOverlayUIView>(_battleUIAssetsDb.UnitUIOverlayView, position,
                Quaternion.identity, parent);

        public async UniTask<GameObject> CreateOverlayDiceFacetAsync(DiceSide diceSide, Transform parent)
            => await _assetsProvider.CreateInstanceAsync(_battleUIAssetsDb.GetOverlayFacetIcon(diceSide), parent);

        public async UniTask<BattleEndUIView> CreateEndBattleUIViewAsync(Transform parent)
            => await _assetsProvider.CreateInstanceAsync<BattleEndUIView>(_battleUIAssetsDb.BattleEndUIView, parent);

        public async UniTask<BattleUnitsOverlayUIView> CreateUnitsOverlayUIViewAsync(Transform parent)
            => await _assetsProvider.CreateInstanceAsync<BattleUnitsOverlayUIView>(_battleUIAssetsDb.BattleUnitsOverlayUIView, parent);

        public async UniTask<BattleDicesUIView> CreateBattleDicesUIViewAsync(Transform parent)
            => await _assetsProvider.CreateInstanceAsync<BattleDicesUIView>(_battleUIAssetsDb.BattleDicesUIView, parent);
    }
}