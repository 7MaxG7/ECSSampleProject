using CustomTypes;

namespace Utils
{
    public static class DicesExtensions
    {
        public static bool IsDamageSide(this DiceSideType sideType)
            => sideType is DiceSideType.MeleeAttack or DiceSideType.RangeAttack;
    }
}