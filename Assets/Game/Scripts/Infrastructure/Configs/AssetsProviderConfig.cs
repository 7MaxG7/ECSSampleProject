using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Infrastructure
{
    [CreateAssetMenu(menuName = "Configs/" + nameof(AssetsProviderConfig), fileName = nameof(AssetsProviderConfig), order = 1)]
    public class AssetsProviderConfig : ScriptableObject
    {
        [SerializeField] private AssetReference[] _battleSceneAssets;
   
        public IEnumerable<AssetReference> GetAssetReferencesForScene(string sceneName)
        {
            return sceneName switch
            {
                Constants.BATTLE_SCENE_NAME => _battleSceneAssets,
                _ => Array.Empty<AssetReference>(),
            };
        }
    }
}