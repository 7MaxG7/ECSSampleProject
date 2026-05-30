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
        private readonly Instantiator _instantiator;
        private readonly EcsService _ecsService;

        private Transform _rootCanvas;
        private Transform _overlayCanvas;

        [Inject]
        public BattleUIFactory(EcsService ecsService, BattleUIAssetsDb battleUIAssetsDb, Instantiator instantiator)
        {
            _battleUIAssetsDb = battleUIAssetsDb;
            _instantiator = instantiator;
            _ecsService = ecsService;
        }

        public async UniTask<BattleUIView> CreateBattleUIViewAsync()
        {
            var battleUIView = await _instantiator.CreateAsync<BattleUIView>(_battleUIAssetsDb.BattleUIView);
            if (_rootCanvas == null)
                _rootCanvas = battleUIView.RootContent;

            return battleUIView;
        }

        public async UniTask<DiceUIView> CreateDiceUIViewAsync(int dice, Transform parent)
        {
            var diceUIView = await _instantiator.CreateAsync<DiceUIView>(_battleUIAssetsDb.DiceUIView, parent);
            _ecsService.AddEntityDebugView(diceUIView.gameObject, dice);
            return diceUIView;
        }

        public async UniTask<UnitOverlayUIView> CreateUnitOverlayViewAsync(Vector3 position, Transform parent)
            => await _instantiator.CreateAsync<UnitOverlayUIView>(_battleUIAssetsDb.UnitUIOverlayView, position,
                Quaternion.identity, parent);

        public async UniTask<OverlayDiceFacetUIView> CreateOverlayDiceFacetAsync(Transform parent)
            => await _instantiator.CreateAsync<OverlayDiceFacetUIView>(_battleUIAssetsDb.OverlayDiceFacetUIView, parent);

        public async UniTask<BattleEndUIView> CreateEndBattleUIViewAsync(Transform parent)
            => await _instantiator.CreateAsync<BattleEndUIView>(_battleUIAssetsDb.BattleEndUIView, parent);

        public async UniTask<BattleUnitsOverlayUIView> CreateUnitsOverlayUIViewAsync(Transform parent)
            => await _instantiator.CreateAsync<BattleUnitsOverlayUIView>(_battleUIAssetsDb.BattleUnitsOverlayUIView, parent);

        public async UniTask<BattleDicesUIView> CreateBattleDicesUIViewAsync(Transform parent)
            => await _instantiator.CreateAsync<BattleDicesUIView>(_battleUIAssetsDb.BattleDicesUIView, parent);
    }
}