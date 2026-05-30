using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using static UnityEngine.Object;

namespace Infrastructure
{
    public class AssetsProvider
    {
        private readonly Dictionary<string, AsyncOperationHandle> _loadedAssets = new();
        private readonly List<AsyncOperationHandle> _handles = new();
        private bool _isSceneCleaned = true;
        
        public void Init()
        {
            Addressables.InitializeAsync();
        }
                
        public void OnDispose()
        {
            ClearScene();
        }

        public void ClearScene()
        {
            if (_isSceneCleaned)
                return;

            foreach (var handle in _handles) 
                Addressables.Release(handle);
            _handles.Clear();
            _loadedAssets.Clear();
            _isSceneCleaned = true;
        }

        public async UniTask<GameObject> LoadAsync(AssetReference assetReference)
        {
            _isSceneCleaned = false;

            if (_loadedAssets.TryGetValue(assetReference.AssetGUID, out var loadedHandle))
                return loadedHandle.Result as GameObject;

            var handle = Addressables.LoadAssetAsync<GameObject>(assetReference);
            handle.Completed += resultHandle => _loadedAssets[assetReference.AssetGUID] = resultHandle;
            _handles.Add(handle);
            
            return await handle.Task;
        }
    }
}