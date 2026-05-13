using Battle;
using CustomTypes;
using Cysharp.Threading.Tasks;
using Infrastructure;
using Leopotam.EcsLite;
using UnityEngine;
using Zenject;

namespace Units
{
    public class UnitViewFactory
    {
        private readonly EcsService _ecsService;
        private readonly AssetsProvider _assetsProvider;
        private readonly StaticDataService _dataService;
        private readonly HighlightService _highlightService;
        private readonly BattleAnimatorService _battleAnimatorService;

        private readonly EcsPool<UnitViewComponent> _unitViewPool;

        private Transform _unitsParent;

        [Inject]
        public UnitViewFactory(EcsService ecsService, AssetsProvider assetsProvider, StaticDataService dataService,
            HighlightService highlightService, BattleAnimatorService battleAnimatorService)
        {
            _ecsService = ecsService;
            _assetsProvider = assetsProvider;
            _dataService = dataService;
            _highlightService = highlightService;
            _battleAnimatorService = battleAnimatorService;

            _unitViewPool = ecsService.World.GetPool<UnitViewComponent>();
        }

        public void Init()
        {
            if (_unitsParent == null)
                _unitsParent = new GameObject(Constants.UNITS_PARENT_NAME).transform;
        }

        public async UniTask<UnitView> SpawnUnitAsync(int unit, UnitSpecialization specialization, Vector3 position, Quaternion rotation,
            TeamType team)
        {
            var unitConfig = _dataService.GetAnyUnit(specialization);
            if (unitConfig == null)
            {
                LogService.LogDebug(DebugType.Error, $"Cannot get unit config for specialization {specialization}");
                return null;
            }

            var unitView = await _assetsProvider.CreateInstanceAsync<UnitView>(unitConfig.Prefab, position, rotation, _unitsParent);

            InitComponents(unit, unitView );

            unitView.SelectView.Init(_ecsService.World.PackEntity(unit));
            unitView.SetTeam(team);

            return unitView;
        }

        private void InitComponents(int unit, UnitView unitView)
        {
            ref var unitViewComponent = ref _unitViewPool.Add(unit);
            unitViewComponent.View = unitView;

            var animator = unitView.Animator;
            _battleAnimatorService.InitComponents(unit, animator);

            _ecsService.AddEntityDebugView(unitView.gameObject, unit);
            _highlightService.InitComponents(unit, unitView.Highlight);
        }
    }
}