using Battle;
using Infrastructure;
using Leopotam.EcsLite;
using UnityEngine;
using Zenject;

namespace Dices
{
    public class DiceViewService
    {
        private readonly TeamService _teamService;

        private readonly EcsFilter _diceFilter;
        private readonly EcsPool<DiceViewComponent> _diceViewPool;

        [Inject]
        public DiceViewService(EcsService ecsService, TeamService teamService)
        {
            _teamService = teamService;

            _diceFilter = ecsService.World.Filter<DiceComponent>().Exc<DeadComponent>().End();
            _diceViewPool = ecsService.World.GetPool<DiceViewComponent>();
        }

        public void ToggleDiceUILock(int dice, bool mustLocked)
        {
            ref var diceViewComponent = ref _diceViewPool.Get(dice);
            diceViewComponent.DiceView.SetLocked(mustLocked);
        }

        public void ToggleDiceHide(int dice, bool mustHidden)
        {
            ref var diceViewComponent = ref _diceViewPool.Get(dice);
            diceViewComponent.DiceView.SetHidden(mustHidden);
        }

        public void ActivateCurrentTeamDices()
        {
            foreach (var dice in _diceFilter)
                ToggleDiceHide(dice, !_teamService.IsCurrentTeamEntity(dice));
        }

        public void Die(int dice)
        {
            ref var diceViewComponent = ref _diceViewPool.Get(dice);
            diceViewComponent.DiceView.gameObject.SetActive(false);
            
            if (diceViewComponent.FacetIcon)
            {
                Object.Destroy(diceViewComponent.FacetIcon);
                diceViewComponent.FacetIcon = null;
            }
        }

        public void SetDiceFacetIcon(int dice, GameObject facetIcon)
        {
            ref var diceViewComponent = ref _diceViewPool.Get(dice);
            diceViewComponent.FacetIcon = facetIcon;
        }

        public void RemoveDiceFacetIcon(int dice)
        {
            ref var diceViewComponent = ref _diceViewPool.Get(dice);
            Object.Destroy(diceViewComponent.FacetIcon);
            diceViewComponent.FacetIcon = null;
        }
    }
}