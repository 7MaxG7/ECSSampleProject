using CustomTypes.Enums.Team;
using Cysharp.Threading.Tasks;

namespace Abstractions.UI.Battle
{
    public interface IBattleEndUIModel
    {
        AsyncReactiveProperty<TeamType> Winner { get; }
    }
}