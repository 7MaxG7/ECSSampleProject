using System;
using Battle;
using CustomTypes;
using Leopotam.EcsLite;
using UI;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Dices
{
    public class DiceUIView : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IDragHandler, IBeginDragHandler, IEndDragHandler
    {
        [SerializeField] private GameButtonView _lockButton;
        [SerializeField] private GameObject _lockObj;
        [SerializeField] private GameObject _hideObj;
        [SerializeField] private HighlightView _highlight;

        public event Action<EcsPackedEntity?> OnDicePointed;
        public event Action<EcsPackedEntity?> OnDiceUnpointed;
        public event Action<EcsPackedEntity?> OnDiceDragBegin;
        public event Action<EcsPackedEntity?> OnDiceDragEnd;

        public GameButtonView LockButton => _lockButton;
        public HighlightView Highlight => _highlight;

        private EcsPackedEntity? _dice;

#region Pointer events
        public void OnPointerEnter(PointerEventData _)
            => OnDicePointed?.Invoke(_dice);

        public void OnPointerExit(PointerEventData _)
            => OnDiceUnpointed?.Invoke(_dice);

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
                return;
            
            OnDiceDragBegin?.Invoke(_dice);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
                return;
            
            OnDiceDragEnd?.Invoke(_dice);
        }

        public void OnDrag(PointerEventData _)
        {
        }
#endregion

        public void Init(EcsPackedEntity dice)
        {
            _dice = dice;
        }

        public void UpdateView(DiceData diceData)
        {
            SetCurrentSide(diceData.DiceSide);
            _lockButton.Interactable = diceData.IsInteractable;
        }

        public void SetCurrentSide(DiceSide diceSide)
            => _lockButton.Text = diceSide.ToString();

        public void SetLocked(bool mustLocked)
            => _lockObj.SetActive(mustLocked);

        public void SetHidden(bool mustHidden)
            => _hideObj.SetActive(mustHidden);
    }
}