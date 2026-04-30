using System;
using System.Threading;
using Abstractions.Infrastructure;
using Cysharp.Threading.Tasks;
using Leopotam.EcsLite;
using Zenject;

namespace Infrastructure.Bootstrap
{
    public abstract class SceneRunner : IInitializable, ITickable, IFixedTickable, IDisposable, IDisposeCoordinated
    {
        private readonly EcsService _ecsService;
        private readonly AssetsProvider _assetsProvider;
        private readonly CancellationTokenProvider _tokenProvider;
        private readonly DisposeCoordinator _disposeCoordinator;

        protected IUpdateSystemsInitializer UpdateSystemsInitializer;
        protected IUpdateSystemsInitializer FixedUpdateSystemsInitializer;
        private EcsSystems _updateSystems;
        private EcsSystems _fixedUpdateSystems;

        private bool _isInited;

        protected SceneRunner(EcsService ecsService, AssetsProvider assetsProvider, CancellationTokenProvider tokenProvider,
            DisposeCoordinator disposeCoordinator)
        {
            _ecsService = ecsService;
            _assetsProvider = assetsProvider;
            _tokenProvider = tokenProvider;
            _disposeCoordinator = disposeCoordinator;
        }

        public void Initialize()
        {
            _disposeCoordinator.RegisterCoordinated(this);
            InitAsync().Forget();
        }

        public void Dispose()
        {
            _disposeCoordinator.DisposeCoordinated(this);
        }

        public void Tick()
        {
            if (!_isInited)
                return;

            _updateSystems?.Run();
        }

        public void FixedTick()
        {
            if (!_isInited)
                return;

            _fixedUpdateSystems?.Run();
        }

        public virtual void OnDispose()
        {
            _isInited = false;

            _assetsProvider.ClearScene();
            _ecsService.DestroySceneSystems();
        }

        protected abstract UniTask OnInitAsync(CancellationTokenSource cts);

        private async UniTaskVoid InitAsync()
        {
            using var localCts = _tokenProvider.CreateLocalCts();

            InitSystems();

            await OnInitAsync(localCts);

            _isInited = true;
        }

        private void InitSystems()
        {
            _updateSystems = _ecsService.CreateSystems(false);
            _fixedUpdateSystems = _ecsService.CreateSystems(true);

            UpdateSystemsInitializer.InitSystems(_updateSystems);
            FixedUpdateSystemsInitializer.InitSystems(_fixedUpdateSystems);
        }
    }
}