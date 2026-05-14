using Battle;
using CustomTypes;
using Cysharp.Threading.Tasks;
using Infrastructure;
using Leopotam.EcsLite;
using UI.Battle;
using UnityEngine;
using Zenject;

namespace Dices
{
    public class DiceAimingService
    {
        private readonly TeamService _teamService;
        private readonly InputService _inputService;
        private readonly BattleUIFactory _battleUIFactory;
        private readonly BattleDiceService _battleDiceService;
        private readonly FrameComponentsService _frameComponentsService;

        private readonly EcsPool<TargetSelectedComponent> _targetSelectedPool;
        private readonly EcsPool<DiceAimingViewComponent> _diceAimingViewPool;

        [Inject]
        public DiceAimingService(EcsService ecsService, TeamService teamService, InputService inputService, BattleUIFactory battleUIFactory,
            BattleDiceService battleDiceService, FrameComponentsService frameComponentsService)
        {
            _teamService = teamService;
            _inputService = inputService;
            _battleUIFactory = battleUIFactory;
            _battleDiceService = battleDiceService;
            _frameComponentsService = frameComponentsService;

            _targetSelectedPool = ecsService.World.GetPool<TargetSelectedComponent>();
            _diceAimingViewPool = ecsService.World.GetPool<DiceAimingViewComponent>();
        }

        public void StartDiceAiming(int dice)
        {
            if (_targetSelectedPool.Has(dice) || !_teamService.IsCurrentTeamEntity(dice))
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
            AddAimingComponent(dice, aimView);
        }

        private void StopAimingView(int dice)
        {
            ref var diceAimingComponent = ref _diceAimingViewPool.Get(dice);
            Object.Destroy(diceAimingComponent.DiceAimView.gameObject);
            _diceAimingViewPool.Del(dice);
        }

        private void AddAimingComponent(int dice, DiceAimUIView aimView)
        {
            ref var diceAimingViewComponent = ref _diceAimingViewPool.Add(dice);
            diceAimingViewComponent.DiceAimView = aimView;
            _frameComponentsService.AddEvent<DiceAimingEventComponent>(dice);
        }
    }
}