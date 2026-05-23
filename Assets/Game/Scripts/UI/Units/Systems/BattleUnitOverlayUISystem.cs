using System.Collections.Generic;
using Abstractions;
using Battle;
using CustomTypes;
using Dices;
using Infrastructure;
using Leopotam.EcsLite;
using UI.Battle;
using Units;
using Utils;
using Zenject;

namespace UI.Units
{
    public class BattleUnitOverlayUISystem : IEcsPostRunSystem
    {
        private readonly EcsService _ecsService;
        private readonly BattleDiceService _battleDiceService;
        private readonly HealthService _healthService;

        private readonly EcsFilter _battleAddedEventFilter;
        private readonly EcsFilter _targetedAddedEventFilter;
        private readonly EcsFilter _targetedDeletedEventFilter;
        private readonly EcsFilter _targetedModifiedEventFilter;
        private readonly EcsFilter _deadUnitAddedEventFilter;
        private readonly EcsFilter _unitFilter;
        private readonly EcsFilter _undelayedUnitUIOverlayFilter;
        private readonly EcsPool<UnitComponent> _unitPool;
        private readonly EcsPool<TargetedComponent> _targetedPool;
        private readonly EcsPool<DeadComponent> _deadPool;
        
        private readonly IUnitsOverlayUIModel _unitsOverlayUIModel;
        private readonly HashSet<string> _addedFacetModelIds = new();

        [Inject]
        public BattleUnitOverlayUISystem(EcsService ecsService, BattleUIModel battleUIModel, BattleDiceService battleDiceService,
            FrameComponentsService frameComponentsService, HealthService healthService)
        {
            _ecsService = ecsService;
            _unitsOverlayUIModel = battleUIModel;
            _battleDiceService = battleDiceService;
            _healthService = healthService;

            _battleAddedEventFilter = frameComponentsService.GetAddedEventFilter<BattleComponent>();
            _targetedAddedEventFilter = frameComponentsService.GetAddedEventFilter<TargetedComponent>();
            _targetedDeletedEventFilter = frameComponentsService.GetDeletedEventFilter<TargetedComponent>();
            _targetedModifiedEventFilter = frameComponentsService.GetModifiedEventFilter<TargetedComponent>();
            _deadUnitAddedEventFilter = frameComponentsService.GetAddedEventMask<DeadComponent>().Inc<UnitComponent>().End();
            _unitFilter = ecsService.World.Filter<UnitComponent>().End();
            _undelayedUnitUIOverlayFilter = ecsService.World.Filter<UnitComponent>().Exc<ViewUpdateDelayComponent>().End();
            _unitPool = ecsService.World.GetPool<UnitComponent>();
            _targetedPool = ecsService.World.GetPool<TargetedComponent>();
            _deadPool = ecsService.World.GetPool<DeadComponent>();
        }

        public void PostRun(IEcsSystems systems)
        {
            UpdateOverlays();
            UpdateDicesFacets();
        }

        private void UpdateOverlays()
        {
            var isUnitModelAdded = false;
            if (_battleAddedEventFilter.GetEntitiesCount() > 0)
                foreach (var unit in _unitFilter)
                    UpdateUnitOverlay(unit, ref isUnitModelAdded);
            
            foreach (var unit in _deadUnitAddedEventFilter)
                UpdateUnitOverlay(unit, ref isUnitModelAdded);

            if (isUnitModelAdded)
                _unitsOverlayUIModel.AreUnitOverlayModelsAdded.Invoke();

            foreach (var unit in _undelayedUnitUIOverlayFilter)
                UpdateHealthBar(unit);
        }

        private void UpdateDicesFacets()
        {
            _addedFacetModelIds.Clear();
            foreach (var unit in _targetedAddedEventFilter)
                UpdateTargetedFacets(unit);
            foreach (var unit in _targetedDeletedEventFilter)
                UpdateTargetedFacets(unit);
            foreach (var unit in _targetedModifiedEventFilter)
                UpdateTargetedFacets(unit);

            foreach (var id in _addedFacetModelIds)
                _unitsOverlayUIModel.UnitOverlayModels[id].AreFacetModelsAdded.Invoke();
        }

        private void UpdateUnitOverlay(int unit, ref bool isUnitModelAdded)
        {
            ref var unitComponent = ref _unitPool.Get(unit);
            isUnitModelAdded |= _unitsOverlayUIModel.UnitOverlayModels.TryAdd(unitComponent.Id, new(unitComponent.Id));
                  
            var overlayModel = _unitsOverlayUIModel.UnitOverlayModels[unitComponent.Id];
            overlayModel.IsOverlayVisible.Update(!_deadPool.Has(unit));
            UpdateHealthBar(unit);
        }

        private void UpdateHealthBar(int unit)
        {
            ref var unitComponent = ref _unitPool.Get(unit);
            
            var overlayModel = _unitsOverlayUIModel.UnitOverlayModels[unitComponent.Id];
            overlayModel.CurrentHp.Update(_healthService.GetCurrentHp(unit).Ceiling());
            overlayModel.MaxHp.Update(_healthService.GetMaxHp(unit));
            overlayModel.Armor.Update(_healthService.GetArmor(unit));
            overlayModel.IncomingDamage.Update(GetIncomingDamage(unit));
        }

        private void UpdateTargetedFacets(int unit)
        {
            ref var unitComponent = ref _unitPool.Get(unit);
            var overlayModel = _unitsOverlayUIModel.UnitOverlayModels[unitComponent.Id];

            if (_targetedPool.Has(unit))
            {
                ref var targetedComponent = ref _targetedPool.Get(unit);
                var index = 0;
                foreach (var dicePacked in targetedComponent.TargetedDices)
                {
                    if (!_ecsService.TryUnpack(dicePacked, out var dice))
                        continue;

                    while (index >= overlayModel.FacetModels.Count)
                    {
                        overlayModel.FacetModels.Add(new());
                        _addedFacetModelIds.Add(unitComponent.Id);
                    }

                    var model = overlayModel.FacetModels[index++];
                    model.DiceSideType.UpdateEnum(_battleDiceService.GetCurrentSide(dice).SideType);
                    model.IsVisible.Update(true);
                }

                for (var i = index; i < overlayModel.FacetModels.Count; i++)
                    overlayModel.FacetModels[i].IsVisible.Update(false);
            }
            else
                foreach (var model in overlayModel.FacetModels)
                    model.IsVisible.Update(false);
        }

        private int GetIncomingDamage(int target)
        {
            if (!_targetedPool.Has(target))
                return 0;
            
            var damage = 0;
            var armor = 0;
            
            ref var targetedComponent = ref _targetedPool.Get(target);
            foreach (var dicePacked in targetedComponent.TargetedDices)
            {
                if (!_ecsService.TryUnpackWithWarning(dicePacked, out var targetedDice))
                    continue;

                var currentSide = _battleDiceService.GetCurrentSide(targetedDice);
                if (currentSide.SideType.IsDamageSide())
                {
                    if (currentSide.Value >= armor)
                    {
                        damage += currentSide.Value - armor;
                        armor = 0;
                    }
                    else
                        armor -= currentSide.Value;

                    var unitHealth = _healthService.GetCurrentHp(target).Ceiling() + _healthService.GetArmor(target);
                    if (unitHealth > damage)
                        continue;
                    
                    damage = unitHealth;
                    break;
                }

                if (currentSide.SideType is DiceSideType.Armor)
                    armor += currentSide.Value;
            }

            return damage;
        }
    }
}