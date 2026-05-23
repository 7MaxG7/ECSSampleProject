using System.Collections.Generic;
using Infrastructure;
using UI.Units;

namespace Abstractions
{
    public interface IUnitsOverlayUIModel
    {
        AsyncReactiveTrigger AreUnitOverlayModelsAdded { get; }
        public Dictionary<string, UnitUIOverlayModel> UnitOverlayModels { get; }
    }
}