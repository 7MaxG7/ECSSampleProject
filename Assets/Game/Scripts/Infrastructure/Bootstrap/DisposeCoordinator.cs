using System.Collections.Generic;
using Abstractions;
using CustomTypes;

namespace Infrastructure
{
    public class DisposeCoordinator
    {
        private readonly Stack<IDisposeCoordinated> _disposeCoordinated = new();

        public void RegisterCoordinated(IDisposeCoordinated coordinated)
        {
            if (_disposeCoordinated.Contains(coordinated))
            {
                LogService.LogDebug(DebugType.Error, $"Dispose coordinated {coordinated.GetType().Name} is already registered");
                return;
            }
            
            _disposeCoordinated.Push(coordinated);
        }

        public void DisposeCoordinated(IDisposeCoordinated coordinated)
        {
            if (!_disposeCoordinated.TryPop(out var lastCoordinated))
                return;

            if (lastCoordinated != coordinated)
            {
                LogService.LogDebug(DebugType.Error, $"Last coordinated is not {coordinated.GetType().Name} (it's {lastCoordinated.GetType().Name})");
                _disposeCoordinated.Push(lastCoordinated);
            }
            
            coordinated.OnDispose();
        }

        public void DisposeAll()
        {
            while (_disposeCoordinated.TryPop(out var coordinated))
                coordinated.OnDispose();
        }
    }
}