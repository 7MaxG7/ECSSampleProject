using UnityEngine;

namespace Infrastructure
{
    public class Instantiator
    {
        public T Create<T>(T gameObject, Transform parent = null) where T : MonoBehaviour
            => Object.Instantiate(gameObject, parent);
    }
}