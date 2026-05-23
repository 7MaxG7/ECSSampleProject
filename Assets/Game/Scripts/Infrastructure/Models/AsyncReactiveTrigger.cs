using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;

namespace Infrastructure
{
    public class AsyncReactiveTrigger : AsyncReactiveProperty<Null>
    {
        public AsyncReactiveTrigger() : base(Null.Default)
        {
        }

        public void Subscribe(Func<CancellationToken, UniTaskVoid> action, CancellationToken cancellationToken)
            => WithoutCurrent().Subscribe((_, token) =>
            {
                action.Invoke(token).Forget();
                return default;
            }, cancellationToken);

        public void Subscribe(Action<CancellationToken> action, CancellationToken cancellationToken)
            => WithoutCurrent().Subscribe((_, token) =>
            {
                action.Invoke(token);
                return default;
            }, cancellationToken);

        public void Invoke()
            => Value = Null.Default;
    }
}