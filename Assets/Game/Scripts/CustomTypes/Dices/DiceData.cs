namespace CustomTypes
{
    public class DiceData
    {
        public DiceSide DiceSide { get; }
        public int Unit { get; }
        public bool IsInteractable { get; }

        public DiceData(DiceSide diceSide, int unit, bool isInteractable)
        {
            DiceSide = diceSide;
            Unit = unit;
            IsInteractable = isInteractable;
        }
    }
}