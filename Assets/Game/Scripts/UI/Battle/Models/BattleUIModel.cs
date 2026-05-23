using System.Collections.Generic;
using Abstractions;
using CustomTypes;
using Cysharp.Threading.Tasks;
using Infrastructure;
using UI.Units;

namespace UI.Battle
{
    public class BattleUIModel : IBattleEndUIModel, IBattleDicesUIModel, IUnitsOverlayUIModel
    {
        public AsyncReactiveProperty<bool> IsRollUIInteractable { get; } = new(default);
        public AsyncReactiveProperty<bool> AreAllTeamDicesLocked { get; } = new(default);
        public Dictionary<TeamType, AsyncReactiveProperty<int>> DiceRolls { get; } = new()
        {
            [TeamType.Player] = new AsyncReactiveProperty<int>(default),
            [TeamType.Enemy] = new AsyncReactiveProperty<int>(default),
        };

        // IBattleDicesUIModel
        public AsyncReactiveTrigger AreDicesAdded { get; } = new();
        public Dictionary<TeamType, Dictionary<string, BattleDiceUIModel>> DiceModels { get; } = new()
        {
            [TeamType.Player] = new Dictionary<string, BattleDiceUIModel>(),
            [TeamType.Enemy] = new Dictionary<string, BattleDiceUIModel>(),
        };
        
        // IBattleEndUIModel
        public AsyncReactiveProperty<bool> IsBattleEndUIVisible { get; } = new(default);
        public AsyncReactiveProperty<TeamType> Winner { get; } = new(default);
        
        // IUnitsOverlayUIModel
        public AsyncReactiveTrigger AreUnitOverlayModelsAdded { get; } = new();
        public Dictionary<string, UnitUIOverlayModel> UnitOverlayModels { get; } = new();
    }
}