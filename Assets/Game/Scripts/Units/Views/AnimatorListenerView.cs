using System;
using Abstractions.Infrastructure;
using Leopotam.EcsLite;
using UnityEngine;

namespace Units.Views
{
    public class AnimatorListenerView : MonoBehaviour, IAnimatorListener
    {
        public event Action<EcsPackedEntity, int> OnStateEnter;
        public event Action<EcsPackedEntity, int> OnStateExit;
        public event Action<EcsPackedEntity> OnActionAnimationEvent;

        private EcsPackedEntity _unitPacked;
        
        public void Init(EcsPackedEntity unitPacked)
        {
            _unitPacked = unitPacked;
        }

        public void EnterState(int nameHash)
            => OnStateEnter?.Invoke(_unitPacked, nameHash);

        public void ExitState(int nameHash)
            => OnStateExit?.Invoke(_unitPacked, nameHash);

        /// <summary>
        /// Animation event
        /// </summary>
        public void ActionAnimationEvent()
            => OnActionAnimationEvent?.Invoke(_unitPacked);
    }
}