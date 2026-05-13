using Battle;
using CustomTypes;
using Cysharp.Threading.Tasks;
using Dices.Events;
using Infrastructure;
using Infrastructure.Input;
using Leopotam.EcsLite;
using UI.Battle;
using UnityEngine;
using Zenject;

namespace Dices
{
    public class DiceAimingService
    {
        private readonly EcsService _ecsService;
        private readonly TeamService _teamService;
        private readonly InputService _inputService;
        private readonly BattleUIFactory _battleUIFactory;
        private readonly BattleDiceService _battleDiceService;

        private readonly EcsPool<TargetSelectedComponent> _targetSelectedPool;
        private readonly EcsPool<DiceAimingViewComponent> _diceAimingViewPool;
        private readonly EcsPool<DiceAimingEventComponent> _diceAimingEventPool;

        [Inject]
        public DiceAimingService(EcsService ecsService, TeamService teamService, InputService inputService, BattleUIFactory battleUIFactory,
            BattleDiceService battleDiceService)
        {
            _ecsService = ecsService;
            _teamService = teamService;
            _inputService = inputService;
            _battleUIFactory = battleUIFactory;
            _battleDiceService = battleDiceService;

            _targetSelectedPool = ecsService.World.GetPool<TargetSelectedComponent>();
            _diceAimingViewPool = ecsService.World.GetPool<DiceAimingViewComponent>();
            _diceAimingEventPool = ecsService.World.GetPool<DiceAimingEventComponent>();
        }

        public void StartDiceAiming(EcsPackedEntity? dicePacked)
        {
            if (!_ecsService.TryUnpack(dicePacked, out var dice) || _targetSelectedPool.Has(dice) ||
                !_teamService.IsCurrentTeamEntity(dice))
                return;

            CreateDiceAim(dice, _battleDiceService.GetCurrentSide(dice), _inputService.MousePosition).Forget();
        }

        public bool TryStopAiming(int dice)
        {
            if (!_diceAimingViewPool.Has(dice))
                return false;

            StopAimingView(dice);
            return true;
        }

        private async UniTaskVoid CreateDiceAim(int dice, DiceSide currentSide, Vector3 position)
        {
            var aimView = await _battleUIFactory.CreateDiceAimViewAsync(currentSide, position);
            _diceAimingViewPool.Add(dice) = new DiceAimingViewComponent
            {
                DiceAimView = aimView,
            };
            _diceAimingEventPool.Add(dice);
        }

        private void StopAimingView(int dice)
        {
            ref var diceAimingComponent = ref _diceAimingViewPool.Get(dice);
            Object.Destroy(diceAimingComponent.DiceAimView.gameObject);
            _diceAimingViewPool.Del(dice);
        }
    }
}