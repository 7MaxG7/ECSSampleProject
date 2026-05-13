using System.Collections.Generic;
using CustomTypes;
using CustomTypes.Enums.Team;
using Cysharp.Threading.Tasks;

namespace Abstractions.UI.Battle
{
    public interface IBattleDicesUIModel
    {
        AsyncReactiveProperty<(TeamType, List<DiceData>)> PlayerMainDices { get; }
        AsyncReactiveProperty<(TeamType, List<DiceData>)> EnemyMainDices { get; }
    }
}