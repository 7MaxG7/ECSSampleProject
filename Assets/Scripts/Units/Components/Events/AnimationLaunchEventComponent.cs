using Abstractions.Infrastructure;
using CustomTypes.Enums;
using CustomTypes.Enums.Battle;

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