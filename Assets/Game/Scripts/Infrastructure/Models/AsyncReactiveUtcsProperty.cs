using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using UnityEngine;

namespace Infrastructure
{
    public class AsyncReactiveUtcsProperty<T> : AsyncReactiveProperty<T>
    {
        private UniTaskCompletionSource _completionSource;
        private bool _hasSubscription;
        private bool _isUpdating;

        public AsyncReactiveUtcsProperty(T value) : base(value)
        {
        }

        public void SubscribeUts(Func<T, CancellationToken, UniTask> action, CancellationToken cancellationToken)
        {
            if (_hasSubscription)
            {
                Debug.LogError("AsyncReactiveUtcsProperty already has subscription - only one is supported");
                return;
            }
            _hasSubscription = true;
            
            this.Subscribe(async (value, token) =>
            {
                await action.Invoke(value, token);
                _completionSource?.TrySetResult();
            }, cancellationToken);
        }

        public async UniTask UpdateAsync(T value)
        {
            if (EqualityComparer<T>.Default.Equals(Value, value))
                return;

            if (!_hasSubscription)
            {
                Value = value;
                return;
            }
            
            if (_isUpdating)
            {
                Debug.LogWarning("AsyncReactiveUtcsProperty already is updating");
                return;
            }
            
            _isUpdating = true;
            _completionSource = new UniTaskCompletionSource();
            Value = value;
            await _completionSource.Task;
            _isUpdating = false;
        }
    }
}