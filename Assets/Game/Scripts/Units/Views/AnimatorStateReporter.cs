using Abstractions.Infrastructure;
using UnityEngine;

namespace Units.Views
{
    public class AnimatorStateReporter : StateMachineBehaviour
    {
        private IAnimatorListener _animatorListener;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateEnter(animator, stateInfo, layerIndex);

            var animatorListener = GetAnimatorListener(animator);
            animatorListener.EnterState(stateInfo.shortNameHash);
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateExit(animator, stateInfo, layerIndex);

            var animatorListener = GetAnimatorListener(animator);
            animatorListener.ExitState(stateInfo.shortNameHash);
        }

        private IAnimatorListener GetAnimatorListener(Animator animator)
            => _animatorListener ??= animator.GetComponent<IAnimatorListener>();
    }
}