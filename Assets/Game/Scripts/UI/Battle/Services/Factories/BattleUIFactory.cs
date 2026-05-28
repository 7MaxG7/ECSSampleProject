using Cysharp.Threading.Tasks;
using Dices;
using Infrastructure;
using UI.Units;
using UnityEngine;
using Zenject;

namespace UI.Battle
{
    public class BattleUIFactory
    {
        private readonly BattleUIAssetsDb _battleUIAssetsDb;
        private readonly AssetsProvider _assetsProvider;
        private readonly EcsService _ecsService;

        private Transform _rootCanvas;
        private Transform _overlayCanvas;

        [Inject]
        public BattleUIFactory(EcsService ecsService, AssetsProvider assetsProvider, BattleUIAssetsDb battleUIAssetsDb)
        {
            _battleUIAssetsDb = battleUIAssetsDb;
            _assetsProvider = assetsProvider;
            _ecsService = ecsService;
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
            _ecsService.AddEntityDebugView(diceUIView.gameObject, dice);
            return diceUIView;
        }

        public async UniTask<UnitOverlayUIView> CreateUnitOverlayViewAsync(Vector3 position, Transform parent)
            => await _assetsProvider.CreateInstanceAsync<UnitOverlayUIView>(_battleUIAssetsDb.UnitUIOverlayView, position,
                Quaternion.identity, parent);

        public async UniTask<OverlayDiceFacetUIView> CreateOverlayDiceFacetAsync(Transform parent)
            => await _assetsProvider.CreateInstanceAsync<OverlayDiceFacetUIView>(_battleUIAssetsDb.OverlayDiceFacetUIView, parent);

        public async UniTask<BattleEndUIView> CreateEndBattleUIViewAsync(Transform parent)
            => await _assetsProvider.CreateInstanceAsync<BattleEndUIView>(_battleUIAssetsDb.BattleEndUIView, parent);

        public async UniTask<BattleUnitsOverlayUIView> CreateUnitsOverlayUIViewAsync(Transform parent)
            => await _assetsProvider.CreateInstanceAsync<BattleUnitsOverlayUIView>(_battleUIAssetsDb.BattleUnitsOverlayUIView, parent);

        public async UniTask<BattleDicesUIView> CreateBattleDicesUIViewAsync(Transform parent)
            => await _assetsProvider.CreateInstanceAsync<BattleDicesUIView>(_battleUIAssetsDb.BattleDicesUIView, parent);
    }
}