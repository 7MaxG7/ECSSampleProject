using CustomTypes;
using Leopotam.EcsLite;
using UnityEngine;

namespace Battle
{
    public class BattleSelectView : MonoBehaviour
    {
        [SerializeField] private BattleSelectionType _selectionType;
        
        public BattleSelectionType SelectionType => _selectionType;
        public EcsPackedEntity Entity { get; private set; }

        public void Init(EcsPackedEntity packedEntity)
        {
            Entity = packedEntity;
        }
    }
}