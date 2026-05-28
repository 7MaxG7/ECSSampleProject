using System.Collections.Generic;
using CustomTypes;
using Cysharp.Threading.Tasks;
using Infrastructure;
using UI.Battle;
using UnityEngine;

namespace Abstractions
{
    public interface IBattleDicesUIModel
    {
        AsyncReactiveTrigger AreDicesAdded { get; }
        Dictionary<TeamType, Dictionary<string, BattleDiceUIModel>> DiceModels { get; }
        AsyncReactiveProperty<bool> IsDiceAimingVisible { get; }
        AsyncReactiveProperty<Vector2> AimingPosition { get; }
        AsyncReactiveProperty<DiceSide> AimingSide { get; }
        AsyncReactiveProperty<TeamType> AimingTeam { get; }
    }
}