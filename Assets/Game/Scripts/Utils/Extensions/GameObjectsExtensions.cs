using UnityEngine;

namespace Utils
{
    public static class GameObjectsExtensions
    {
        public static void UpdateActive(this GameObject gameObject, bool isActive)
        {
            if (gameObject != null && gameObject.activeSelf != isActive)
                gameObject.SetActive(isActive);
        }
    }
}