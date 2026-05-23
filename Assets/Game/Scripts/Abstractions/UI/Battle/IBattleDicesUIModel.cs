using System.Collections.Generic;
using CustomTypes;
using Cysharp.Threading.Tasks;
using Infrastructure;
using UI.Battle;

namespace Abstractions
{
    public interface IBattleDicesUIModel
    {
        AsyncReactiveProperty<bool> AreAllTeamDicesLocked { get; }
        AsyncReactiveTrigger AreDicesAdded { get; }
        Dictionary<TeamType, Dictionary<string, BattleDiceUIModel>> DiceModels { get; }
    }
}