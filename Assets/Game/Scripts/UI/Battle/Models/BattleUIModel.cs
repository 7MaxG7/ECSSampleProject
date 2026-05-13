using System.Collections.Generic;
using Abstractions.UI.Battle;
using CustomTypes;
using CustomTypes.Enums.Team;
using Cysharp.Threading.Tasks;

namespace UI.Battle
{
    public class BattleUIModel : IBattleEndUIModel, IBattleDicesUIModel
    {
        public AsyncReactiveProperty<(TeamType, int)> PlayerRollsCount { get; } = new(default);
        public AsyncReactiveProperty<(TeamType, int)> EnemyRollsCount { get; } = new(default);

        // IBattleDicesUIModel
        public AsyncReactiveProperty<(TeamType, List<DiceData>)> PlayerMainDices { get; } = new(default);
        public AsyncReactiveProperty<(TeamType, List<DiceData>)> EnemyMainDices { get; } = new(default);
        
        // IBattleEndUIModel
        public AsyncReactiveProperty<TeamType> Winner { get; } = new(default);
    }
}