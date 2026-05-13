using Cysharp.Threading.Tasks;
using Dices;
using Infrastructure;
using Leopotam.EcsLite;
using Zenject;

namespace Battle
{
    public class BattleTargetSelectStateSystem : IEcsRunSystem
    {
        private readonly DiceTargetSelectService _targetSelectService;
        private readonly TeamService _teamService;
        private readonly BattleStateMachine _battleStateMachine;
        private readonly CancellationTokenProvider _tokenProvider;

        [Inject]
        public BattleTargetSelectStateSystem(DiceTargetSelectService targetSelectService, BattleStateMachine battleStateMachine,
            TeamService teamService, CancellationTokenProvider tokenProvider)
        {
            _targetSelectService = targetSelectService;
            _teamService = teamService;
            _battleStateMachine = battleStateMachine;
            _tokenProvider = tokenProvider;
        }

        public void Run(IEcsSystems systems)
        {
            if (!_targetSelectService.IsTargetSelecting)
                return;

            if (IsTargetingStateCompleted())
            {
                EnterApplyDiceStateAsync().Forget();
                return;
            }

            if (IsEnemyTargetingComplete())
                _targetSelectService.StartTeamTargetSelection(_targetSelectService.SelectionEndTeam);
        }

        private bool IsTargetingStateCompleted()
            => _teamService.IsCurrentTeam(_targetSelectService.SelectionEndTeam) && _targetSelectService.IsCurrentTeamTargetingFinished();

        private bool IsEnemyTargetingComplete()
            => _teamService.IsCurrentTeam(_targetSelectService.SelectionStartTeam) && _targetSelectService.IsCurrentTeamTargetingFinished();

        private async UniTaskVoid EnterApplyDiceStateAsync()
        {
            using var localCts = _tokenProvider.CreateLocalCts();

            // TODO. Mb need another way to wait frame for ui update (asking ui if it's updated)
            await UniTask.NextFrame(localCts.Token);
            _battleStateMachine.Enter<DicesApplyState>();
        }
    }
}