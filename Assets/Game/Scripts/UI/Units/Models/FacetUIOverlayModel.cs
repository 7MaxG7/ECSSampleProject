using CustomTypes;
using Cysharp.Threading.Tasks;

namespace UI.Units
{
    public class FacetUIOverlayModel
    {
        public AsyncReactiveProperty<bool> IsVisible { get; } = new(default);
        public AsyncReactiveProperty<DiceSideType> DiceSideType { get; } = new(default);
    }
}