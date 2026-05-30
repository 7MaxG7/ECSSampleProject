using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Zenject;

namespace Infrastructure
{
    public class Instantiator
    {
        private readonly AssetsProvider _assetsProvider;

        [Inject]
        public Instantiator(AssetsProvider assetsProvider)
        {
            _assetsProvider = assetsProvider;
        }
        
        public T Create<T>(T prefab, Transform parent = null) where T : MonoBehaviour
            => Object.Instantiate(prefab, parent);

        public T Create<T>(T prefab, Vector3 position, Quaternion rotation, Transform parent = null) where T : MonoBehaviour
            => Object.Instantiate(prefab, position, rotation, parent);

        public async UniTask<T> CreateAsync<T>(AssetReference assetReference, Transform parent = null) where T : MonoBehaviour
        {
            var prefab = await LoadPrefab<T>(assetReference);
            return Create(prefab, parent);
        }

        public async UniTask<T> CreateAsync<T>(AssetReference assetReference, Vector3 position, Quaternion rotation
            , Transform parent = null) where T : MonoBehaviour
        {
            var prefab = await LoadPrefab<T>(assetReference);
            return Create(prefab, position, rotation, parent);
        }

        private async UniTask<T> LoadPrefab<T>(AssetReference assetReference) where T : MonoBehaviour
        {
            var prefab = await _assetsProvider.LoadAsync(assetReference);
            return prefab.GetComponent<T>();
        }
    }
}