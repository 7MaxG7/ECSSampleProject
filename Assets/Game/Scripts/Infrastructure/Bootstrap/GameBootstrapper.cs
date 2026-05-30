using Abstractions;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Leopotam.EcsLite;
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
        private readonly StaticDataService _staticDataService;
        private readonly InputService _inputService;
        private readonly PermanentUIBuilder _permanentUIBuilder;
        private readonly DisposeCoordinator _disposeCoordinator;

        private EcsSystems _editorSystems;

        private bool _isInited;

        public GameBootstrapper(AssetsProvider assetsProvider, SceneLoader sceneLoader, EcsService ecsService, RandomService randomService,
            CancellationTokenProvider tokenProvider, GameEditorSystemsInitializer editorSystemsInitializer, InputService inputService,
            StaticDataService staticDataService, PermanentUIBuilder permanentUIBuilder, DisposeCoordinator disposeCoordinator)
        {
            _assetsProvider = assetsProvider;
            _sceneLoader = sceneLoader;
            _ecsService = ecsService;
            _randomService = randomService;
            _tokenProvider = tokenProvider;
            _editorSystemsInitializer = editorSystemsInitializer;
            _staticDataService = staticDataService;
            _inputService = inputService;
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
            _tokenProvider.OnDispose();
            _inputService.OnDispose();
            _permanentUIBuilder.OnDispose();
            DOTween.Clear();
            _assetsProvider.OnDispose();
            _ecsService.DestroyAll();
        }

        private async UniTaskVoid InitInfrastructureAsync()
        {
            // Logic
            _tokenProvider.Init();
            using var localCts = _tokenProvider.CreateLocalCts();

            _ecsService.Init();
            _randomService.Init();
            _staticDataService.Init();
            InitSystemsAsync();
            
            // Views
            DOTween.Init();
            _assetsProvider.Init();
            _permanentUIBuilder.BuildUI();


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