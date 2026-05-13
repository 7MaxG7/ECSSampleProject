using Battle;
using Infrastructure;
using Leopotam.EcsLite;
using Zenject;

namespace Dices
{
    public class BattleDiceLockService
    {
        private readonly TeamService _teamService;
        private readonly EcsPool<LockedComponent> _lockedPool;
        private readonly EcsPool<DiceLockEventComponent> _diceLockEventPool;
        
        private readonly EcsFilter _lockedDiceFilter;
        private readonly EcsFilter _unlockedDiceFilter;

        [Inject]
        public BattleDiceLockService(EcsService ecsService, TeamService teamService)
        {
            _teamService = teamService;
            _lockedPool = ecsService.World.GetPool<LockedComponent>();
            _diceLockEventPool = ecsService.World.GetPool<DiceLockEventComponent>();
            
            _lockedDiceFilter = ecsService.World.Filter<DiceComponent>().Inc<LockedComponent>().Exc<DeadComponent>().End();
            _unlockedDiceFilter = ecsService.World.Filter<DiceComponent>().Exc<LockedComponent>().Exc<DeadComponent>().End();
        }

        public void ToggleDiceLock(int dice)
        {
            var mustLocked = !IsLocked(dice);
            SetDiceLock(dice, mustLocked);
        }

        public bool IsLocked(int dice)
            => _lockedPool.Has(dice);

        public bool AreCurrentTeamDicesLocked()
        {
            foreach (var dice in _unlockedDiceFilter)
                if (_teamService.IsCurrentTeamEntity(dice))
                    return false;

            return true;
        }

        public void UnlockAllDices()
        {
            foreach (var dice in _lockedDiceFilter)
                SetDiceLock(dice, false);
        }

        public void SetCurrentTeamDicesLock(bool mustLocked)
        {
            var diceFilter = mustLocked ? _unlockedDiceFilter : _lockedDiceFilter;
            foreach (var dice in diceFilter)
            {
                if (!_teamService.IsCurrentTeamEntity(dice))
                    continue;

                SetDiceLock(dice, mustLocked);
            }
        }

        private void SetDiceLock(int dice, bool isLocked)
        {
            if (isLocked)
                _lockedPool.Add(dice);
            else
                _lockedPool.Del(dice);

            ref var diceLockEventComponent = ref _diceLockEventPool.Add(dice);
            diceLockEventComponent.IsLocked = isLocked;
        }
    }
}