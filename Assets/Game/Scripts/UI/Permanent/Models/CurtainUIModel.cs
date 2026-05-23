using Infrastructure;

namespace UI.Permanent
{
    public class CurtainUIModel
    {
        public AsyncReactiveUtcsProperty<bool> IsActive { get; } = new(default);

        public bool IsInstantAnimation;
    }
}