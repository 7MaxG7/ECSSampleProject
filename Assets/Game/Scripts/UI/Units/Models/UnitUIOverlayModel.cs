using System.Collections.Generic;
using Abstractions;
using Cysharp.Threading.Tasks;
using Infrastructure;

namespace UI.Units
{
    public class UnitUIOverlayModel : IHealthBarUIModel
    {

        public AsyncReactiveProperty<bool> IsOverlayVisible { get; } = new(default);
        public AsyncReactiveTrigger AreFacetModelsAdded { get; } = new();
        public List<FacetUIOverlayModel> FacetModels { get; } = new();
        
        // IHealthBarUIModel
        public AsyncReactiveProperty<int> CurrentHp { get; } = new(default);
        public AsyncReactiveProperty<int> MaxHp { get; } = new(default);
        public AsyncReactiveProperty<int> Armor { get; } = new(default);
        public AsyncReactiveProperty<int> IncomingDamage { get; } = new(default);
    }
}