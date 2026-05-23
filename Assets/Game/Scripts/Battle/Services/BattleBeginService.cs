using Infrastructure;
using Leopotam.EcsLite;
using Zenject;

namespace Battle
{
    public class BattleBeginService
    {
        public bool IsBattleBeginState { get; private set; }
        
        private readonly EcsService _ecsService;
        private readonly FrameComponentsService _frameComponentsService;

        private readonly EcsPool<BattleComponent> _battlePool;
        private readonly EcsPool<CurrentTeamComponent> _currentTeamPool;

        [Inject]
        public BattleBeginService(EcsService ecsService, FrameComponentsService frameComponentsService)
        {
            _ecsService = ecsService;
            _frameComponentsService = frameComponentsService;

            _battlePool = ecsService.World.GetPool<BattleComponent>();
            _currentTeamPool = ecsService.World.GetPool<CurrentTeamComponent>();
        }
        
        public void StartBattleBegin()
        {
            var battle = _ecsService.CreateEntity();
            _battlePool.Add(battle);
            _currentTeamPool.Add(battle);
            _frameComponentsService.AddAddedEvent<BattleComponent>(battle);
            
            IsBattleBeginState = true;
        }

        public void FinishBattleBegin()
        {
            IsBattleBeginState = false;
        }
    }
}