namespace Abstractions.Infrastructure
{
    public interface IAnimatorListener
    {
        void EnterState(int nameHash);
        void ExitState(int nameHash);
    }
}