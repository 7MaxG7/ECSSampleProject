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

        public event Action<DiceUIView> OnDicePointed;
        public event Action<DiceUIView> OnDiceUnpointed;
        public event Action<DiceUIView> OnDiceDragBegin;
        public event Action<DiceUIView> OnDiceDragEnd;

        public GameButtonView LockButton => _lockButton;
        public HighlightView Highlight => _highlight;

#region Pointer events
        public void OnPointerEnter(PointerEventData _)
            => OnDicePointed?.Invoke(this);

        public void OnPointerExit(PointerEventData _)
            => OnDiceUnpointed?.Invoke(this);

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
                return;
            
            OnDiceDragBegin?.Invoke(this);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
                return;
            
            OnDiceDragEnd?.Invoke(this);
        }

        public void OnDrag(PointerEventData _)
        {
        }
#endregion

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