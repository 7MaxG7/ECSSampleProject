using Abstractions;
using CustomTypes;

namespace Units
{
    /// <summary>
    /// Invokes animation
    /// </summary>
    public struct AnimationLaunchEventComponent : IFrameEvent
    {
        public BattleAnimationType AnimationType;
        public DiceSideType DiceSideType;
    }
}