using Infrastructure;
using Leopotam.EcsLite;
using Units;
using UnityEngine;
using Zenject;

namespace UI.Units
{
    public class UnitOverlayUIService
    {
        private readonly EcsPool<UnitViewComponent> _unitViewPool;

        [Inject]
        public UnitOverlayUIService(EcsService ecsService)
        {
            _unitViewPool = ecsService.World.GetPool<UnitViewComponent>();
        }

        public Transform GetOverlayAnchor(int unit)
        {
            ref var unitViewComponent = ref _unitViewPool.Get(unit);
            return unitViewComponent.View.UIOverlayAnchor;
        }
    }
}