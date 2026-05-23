using Cysharp.Threading.Tasks;

namespace Abstractions
{
    public interface IHealthBarUIModel
    {
        public AsyncReactiveProperty<int> CurrentHp { get; }
        public AsyncReactiveProperty<int> MaxHp { get; }
        public AsyncReactiveProperty<int> Armor { get; }
        public AsyncReactiveProperty<int> IncomingDamage { get; }
    }
}