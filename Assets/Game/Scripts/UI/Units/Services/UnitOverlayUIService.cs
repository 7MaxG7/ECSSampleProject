using CustomTypes;
using Cysharp.Threading.Tasks;
using Infrastructure;
using Leopotam.EcsLite;
using UI.Battle;
using Units;
using UnityEngine;
using Zenject;

namespace UI.Units
{
    public class UnitOverlayUIService
    {
        private readonly BattleUIFactory _battleUIFactory;

        private readonly EcsPool<UnitUIOverlayComponent> _unitUIOverlayPool;
        private readonly EcsPool<UnitViewComponent> _unitViewPool;

        [Inject]
        public UnitOverlayUIService(EcsService ecsService, BattleUIFactory battleUIFactory)
        {
            _battleUIFactory = battleUIFactory;

            _unitUIOverlayPool = ecsService.World.GetPool<UnitUIOverlayComponent>();
            _unitViewPool = ecsService.World.GetPool<UnitViewComponent>();
        }

        public void Clear(int unit)
        {
            _unitUIOverlayPool.Del(unit);
        }

        public async UniTask<GameObject> AddOverlayDiceFacetAsync(int unit, DiceSide diceSide)
        {
            var content = GetOverlayFacetsContent(unit);
            return await _battleUIFactory.CreateOverlayDiceFacetAsync(diceSide, content);
        }

        public void InitComponents(int unit, UnitOverlayUIView unitOverlayUIView)
        {
            ref var unitUIOverlayComponent = ref _unitUIOverlayPool.Add(unit);
            unitUIOverlayComponent.UnitOverlayUIView = unitOverlayUIView;
            unitUIOverlayComponent.OverlayAnchor = GetOverlayAnchor(unit);
        }

        public Transform GetOverlayAnchor(int unit)
        {
            ref var unitViewComponent = ref _unitViewPool.Get(unit);
            return unitViewComponent.View.UIOverlayAnchor;
        }

        private Transform GetOverlayFacetsContent(int unit)
        {
            ref var unitUIOverlayComponent = ref _unitUIOverlayPool.Get(unit);
            return unitUIOverlayComponent.UnitOverlayUIView.FacetIconsContent;
        }
    }
}