using CustomTypes;
using Cysharp.Threading.Tasks;

namespace UI.Battle
{
    public class BattleDiceUIModel
    {
        public AsyncReactiveProperty<bool> IsVisible { get; } = new(default);
        public AsyncReactiveProperty<TeamType> Team { get; } = new(default);
        public AsyncReactiveProperty<bool> IsInteractable { get; } = new(default);
        public AsyncReactiveProperty<bool> IsLocked { get; } = new(default);
        public AsyncReactiveProperty<bool> IsDimmed { get; } = new(default);
        public AsyncReactiveProperty<bool> IsLit { get; } = new(default);
        public AsyncReactiveProperty<bool> IsAiming { get; } = new(default);
        public AsyncReactiveProperty<DiceSide> DiceSide { get; } = new(default);
    }
}