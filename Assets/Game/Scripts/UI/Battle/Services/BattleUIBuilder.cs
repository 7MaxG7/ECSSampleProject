using System.Collections.Generic;
using Battle;
using CustomTypes.Enums.Team;
using Cysharp.Threading.Tasks;
using Infrastructure;
using Leopotam.EcsLite;
using Units;
using Zenject;

namespace UI.Battle
{
    public class BattleUIBuilder
    {
        private readonly BattleUIFactory _battleUIFactory;
        private readonly BattleUIController _battleUIController;

        private readonly EcsFilter _unitFilter;
        private readonly EcsPool<TeamComponent> _teamPool;

        [Inject]
        public BattleUIBuilder(EcsService ecsService, BattleUIFactory battleUIFactory, BattleUIController battleUIController)
        {
            _battleUIFactory = battleUIFactory;
            _battleUIController = battleUIController;

            _unitFilter = ecsService.World.Filter<UnitComponent>().End();
            _teamPool = ecsService.World.GetPool<TeamComponent>();
        }

        public async UniTask BuildBattleUIAsync()
        {
            var battleUIView = await _battleUIFactory.CreateBattleUIViewAsync();
            await _battleUIController.InitAsync(new BattleUIModel(), battleUIView);
            await _battleUIController.CreateUnitsUI(GetUnits());

            _battleUIController.ToggleRollUIInteractable(false);
        }

        public void OnDispose()
        {
            _battleUIController.OnDispose();
        }

        private Dictionary<TeamType, List<int>> GetUnits()
        {
            var units = new Dictionary<TeamType, List<int>>();
            foreach (var unit in _unitFilter)
            {
                ref var teamComponent = ref _teamPool.Get(unit);
                if (!units.ContainsKey(teamComponent.Team))
                    units.Add(teamComponent.Team, new List<int>());

                units[teamComponent.Team].Add(unit);
            }

            return units;
        }
    }
}