using Abstractions;
using Cysharp.Threading.Tasks;
using Leopotam.EcsLite;
using UI;
using UI.Permanent;
using Zenject;

namespace Infrastructure
{
    public class GameBootstrapper : IInitializable, ITickable, ILateDisposable, IDisposeCoordinated
    {
        private readonly AssetsProvider _assetsProvider;
        private readonly SceneLoader _sceneLoader;
        private readonly EcsService _ecsService;
        private readonly RandomService _randomService;
        private readonly CancellationTokenProvider _tokenProvider;
        private readonly GameEditorSystemsInitializer _editorSystemsInitializer;
        private readonly StaticDataService _dataService;
        private readonly InputService _inputService;
        private readonly UiAnimationUtility _uiAnimationUtility;
        private readonly PermanentUIBuilder _permanentUIBuilder;
        private readonly DisposeCoordinator _disposeCoordinator;

        private EcsSystems _editorSystems;

        private bool _isInited;

        public GameBootstrapper(AssetsProvider assetsProvider, SceneLoader sceneLoader, EcsService ecsService, RandomService randomService,
            CancellationTokenProvider tokenProvider, GameEditorSystemsInitializer editorSystemsInitializer, StaticDataService dataService,
            UiAnimationUtility uiAnimationUtility, PermanentUIBuilder permanentUIBuilder, DisposeCoordinator disposeCoordinator,
            InputService inputService)
        {
            _assetsProvider = assetsProvider;
            _sceneLoader = sceneLoader;
            _ecsService = ecsService;
            _randomService = randomService;
            _tokenProvider = tokenProvider;
            _editorSystemsInitializer = editorSystemsInitializer;
            _dataService = dataService;
            _inputService = inputService;
            _uiAnimationUtility = uiAnimationUtility;
            _permanentUIBuilder = permanentUIBuilder;
            _disposeCoordinator = disposeCoordinator;
        }

        public void Initialize()
        {
            _disposeCoordinator.RegisterCoordinated(this);
            InitInfrastructureAsync().Forget();
        }

        public void Tick()
        {
            if (!_isInited)
                return;

            _editorSystems?.Run();
        }

        public void LateDispose()
        {
            _disposeCoordinator.DisposeAll();
        }

        public void OnDispose()
        {
            _permanentUIBuilder.OnDispose();
            _uiAnimationUtility.OnDispose();
            _inputService.OnDispose();
            _tokenProvider.OnDispose();
            _assetsProvider.OnDispose();
            _ecsService.DestroyAll();
        }

        private async UniTaskVoid InitInfrastructureAsync()
        {
            _tokenProvider.Init();
            using var localCts = _tokenProvider.CreateLocalCts();

            _assetsProvider.Init();
            _ecsService.Init();
            _randomService.Init();
            _dataService.Init();
            _uiAnimationUtility.Init();
            _permanentUIBuilder.BuildUI();

            InitSystemsAsync();

            await _sceneLoader.LoadSceneAsync(Constants.BATTLE_SCENE_NAME, localCts);
            _isInited = true;
        }

        private void InitSystemsAsync()
        {
            _editorSystems = _ecsService.CreateSystems(false);

            _editorSystemsInitializer.InitSystems(_editorSystems);
        }
    }
}