namespace Abstractions
{
    public interface IAnimatorListener
    {
        void EnterState(int nameHash);
        void ExitState(int nameHash);
    }
}