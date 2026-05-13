using Cysharp.Threading.Tasks;

namespace Utils.Extensions
{
    public static class UniTaskExtensions
    {
        public static void Update<T>(this AsyncReactiveProperty<T> property, T value) where T : struct
        {
            if (property.Value.IsEqual(value))
                return;

            property.Value = value;
        }

    }
}