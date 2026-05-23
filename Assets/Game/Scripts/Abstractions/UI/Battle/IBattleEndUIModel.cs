using CustomTypes;
using Cysharp.Threading.Tasks;

namespace Abstractions
{
    public interface IBattleEndUIModel
    {
        AsyncReactiveProperty<bool> IsBattleEndUIVisible { get; }
        AsyncReactiveProperty<TeamType> Winner { get; }
    }
}