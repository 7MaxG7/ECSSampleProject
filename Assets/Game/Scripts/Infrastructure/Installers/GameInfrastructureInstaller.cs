using Leopotam.EcsLite.UnityEditor;
using Zenject;

namespace Infrastructure
{
    public class GameInfrastructureInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            // Infrastructure
            Container.Bind<CancellationTokenProvider>().AsSingle();
            Container.Bind<AssetsProvider>().AsSingle();
            Container.Bind<SceneLoader>().AsSingle();
            Container.Bind<EcsService>().AsSingle();
            Container.Bind<RandomService>().AsSingle();
            Container.Bind<LogService>().AsSingle();
            Container.Bind<StaticDataService>().AsSingle();
            Container.Bind<DisposeCoordinator>().AsSingle();
            Container.Bind<Instantiator>().AsSingle();
            Container.Bind<FrameComponentsService>().AsSingle();

            // Debug
            Container.Bind<EcsWorldDebugSystem>().AsSingle().WithArguments(false);
            
            // Initializers
            Container.Bind<GameEditorSystemsInitializer>().AsSingle();
        }
    }
}