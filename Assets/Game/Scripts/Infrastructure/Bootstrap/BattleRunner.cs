using System.Threading;
using Battle;
using Battle.Battlefield;
using Cysharp.Threading.Tasks;
using UI.Battle;
using UI.Permanent;
using Units;
using Zenject;

namespace Infrastructure
{
    public class BattleRunner : SceneRunner
    {
        private readonly BattlefieldBuilder _battlefieldBuilder;
        private readonly UnitViewFactory _unitViewFactory;
        private readonly BattleUIBuilder _battleUIBuilder;
        private readonly BattleAnimatorService _animatorService;
        private readonly CurtainService _curtainService;
        private readonly BattleUpdateSystemsInitializer _updateSystemsInitializer;
        private readonly BattleFixedUpdateSystemsInitializer _fixedUpdateSystemsInitializer;
        private readonly BattleStateMachine _battleStateMachine;
        private readonly BattleSelectionService _battleSelectionService;

        [Inject]
        public BattleRunner(EcsService ecsService, BattleSelectionService battleSelectionService, DisposeCoordinator disposeCoordinator,
            BattleUpdateSystemsInitializer updateSystemsInitializer, BattleFixedUpdateSystemsInitializer fixedUpdateSystemsInitializer,
            BattleStateMachine battleStateMachine, AssetsProvider assetsProvider, BattlefieldBuilder battlefieldBuilder,
            UnitViewFactory unitViewFactory, BattleUIBuilder battleUIBuilder, BattleAnimatorService animatorService,
            CurtainService curtainService, CancellationTokenProvider tokenProvider, AssetsProviderConfig assetsProviderConfig,
            SceneLoader sceneLoader) : base(ecsService, assetsProvider, tokenProvider, disposeCoordinator, sceneLoader,
            assetsProviderConfig)
        {
            _battlefieldBuilder = battlefieldBuilder;
            _unitViewFactory = unitViewFactory;
            _battleUIBuilder = battleUIBuilder;
            _animatorService = animatorService;
            _curtainService = curtainService;
            UpdateSystemsInitializer = updateSystemsInitializer;
            FixedUpdateSystemsInitializer = fixedUpdateSystemsInitializer;
            _battleStateMachine = battleStateMachine;
            _battleSelectionService = battleSelectionService;
        }

        protected override async UniTask OnInitAsync(CancellationTokenSource cts)
        {
            _unitViewFactory.Init();

            await _battlefieldBuilder.BuildBattlefieldAsync();
            await _battleUIBuilder.BuildBattleUIAsync();
            _battleSelectionService.Init();

            _battleStateMachine.Enter<BattleBeginState>();
            await _curtainService.Hide();
        }

        public override void OnDispose()
        {
            base.OnDispose();

            _animatorService.OnDispose();
            _battleSelectionService.OnDispose();
            _battleUIBuilder.OnDispose();
            _battlefieldBuilder.OnDispose();
        }
    }
}