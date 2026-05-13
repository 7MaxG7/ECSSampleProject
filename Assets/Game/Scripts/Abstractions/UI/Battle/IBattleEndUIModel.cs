using CustomTypes;
using Cysharp.Threading.Tasks;

namespace Abstractions
{
    public interface IBattleEndUIModel
    {
        AsyncReactiveProperty<TeamType> Winner { get; }
    }
}