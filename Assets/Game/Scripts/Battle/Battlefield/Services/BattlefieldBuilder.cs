using Cysharp.Threading.Tasks;
using Infrastructure;
using Leopotam.EcsLite;
using Units;
using Utils;
using Zenject;

namespace Battle.Battlefield
{
    public class BattlefieldBuilder
    {
        private readonly EcsService _ecsService;
        private readonly BattlefieldViewFactory _battlefieldViewFactory;
        private readonly UnitSpawner _unitSpawner;
        private readonly TeamBuilder _teamBuilder;
        private readonly BattlefieldFactory _battlefieldFactory;

        private readonly EcsFilter _unitFilter;
        private readonly EcsFilter _battlefieldFilter;

        [Inject]
        public BattlefieldBuilder(EcsService ecsService, BattlefieldViewFactory battlefieldViewFactory, UnitSpawner unitSpawner,
            TeamBuilder teamBuilder, BattlefieldFactory battlefieldFactory)
        {
            _ecsService = ecsService;
            _battlefieldViewFactory = battlefieldViewFactory;
            _unitSpawner = unitSpawner;
            _teamBuilder = teamBuilder;
            _battlefieldFactory = battlefieldFactory;

            _unitFilter = ecsService.World.Filter<UnitComponent>().End();
            _battlefieldFilter = ecsService.World.Filter<BattlefieldComponent>().End();
        }

        public async UniTask BuildBattlefieldAsync()
        {
            var battlefield = _battlefieldFactory.CreateBattlefield();
            await _battlefieldViewFactory.CreateBattlefieldViewAsync(battlefield);
            _teamBuilder.BuildTeams();
            foreach (var unit in _unitFilter)
                await _unitSpawner.SpawnUnitAsync(unit);
        }

        public void OnDispose()
        {
            if (_ecsService.IsInited())
                _ecsService.DestroyEntity(_battlefieldFilter.GetSingle());
            
            foreach (var unit in _unitFilter)
                _unitSpawner.DespawnUnit(unit);
        }
    }
}