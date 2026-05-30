using Battle.Battlefield;
using CustomTypes;
using Cysharp.Threading.Tasks;
using Infrastructure;
using Leopotam.EcsLite;
using Zenject;

namespace Battle
{
    public class BattlefieldViewFactory
    {
        private readonly BattlefieldViewConfig _battlefieldViewConfig;
        private readonly BattleCellService _cellService;
        private readonly Instantiator _instantiator;
        private readonly BattlefieldViewService _battlefieldViewService;

        private readonly EcsPool<BattlefieldViewComponent> _battlefieldViewPool;

        [Inject]
        public BattlefieldViewFactory(EcsService ecsService, BattlefieldViewService battlefieldViewService, BattleCellService cellService,
            BattlefieldViewConfig battlefieldViewConfig, Instantiator instantiator)
        {
            _battlefieldViewConfig = battlefieldViewConfig;
            _cellService = cellService;
            _instantiator = instantiator;
            _battlefieldViewService = battlefieldViewService;

            _battlefieldViewPool = ecsService.World.GetPool<BattlefieldViewComponent>();
        }

        public async UniTask CreateBattlefieldViewAsync(int battlefield)
        {
            var battlefieldView = await _instantiator.CreateAsync<BattlefieldView>(_battlefieldViewConfig.BattlefieldPref);
            InitViewComponent(battlefield, battlefieldView);

            foreach (var cell in _cellService.GetCells(battlefield))
                _battlefieldViewService.SetTile(cell.Location, TileState.Inactive);
        }

        private void InitViewComponent(int battlefield, BattlefieldView battlefieldView)
        {
            ref var battlefieldViewComponent = ref _battlefieldViewPool.Add(battlefield);
            battlefieldViewComponent.BattlefieldView = battlefieldView;
        }
    }
}