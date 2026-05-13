using System.Collections.Generic;
using CustomTypes;
using Cysharp.Threading.Tasks;

namespace Abstractions
{
    public interface IBattleDicesUIModel
    {
        AsyncReactiveProperty<(TeamType, List<DiceData>)> PlayerMainDices { get; }
        AsyncReactiveProperty<(TeamType, List<DiceData>)> EnemyMainDices { get; }
    }
}