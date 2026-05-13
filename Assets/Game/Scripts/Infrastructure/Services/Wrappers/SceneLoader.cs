using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;

namespace Infrastructure
{
    public sealed class SceneLoader
    {
        public async UniTask LoadSceneAsync(string sceneName, CancellationTokenSource cts)
        {
            if (GetCurrentSceneName() == sceneName)
                return;

            await SceneManager.LoadSceneAsync(sceneName).WithCancellation(cts.Token);
        }

        public string GetCurrentSceneName() 
            => SceneManager.GetActiveScene().name;
    }
}