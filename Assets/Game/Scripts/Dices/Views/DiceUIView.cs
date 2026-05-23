using System;
using Battle;
using CustomTypes;
using UI;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Dices
{
    public class DiceUIView : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IDragHandler, IBeginDragHandler, IEndDragHandler
    {
        [SerializeField] private GameButtonView _lockButton;
        [SerializeField] private GameObject _lockObj;
        [SerializeField] private GameObject _dimmer;
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

        public void SetCurrentSide(DiceSide diceSide)
        {
            var text = diceSide.SideType.ToString();
            if (diceSide.IsValued)
                text += $"\n{diceSide.Value}";
            _lockButton.Text = text;

        }

        public void SetInteractable(bool isInteractable)
            => _lockButton.Interactable = isInteractable;

        public void SetLocked(bool mustLocked)
            => _lockObj.SetActive(mustLocked);

        public void SetDimmed(bool mustHidden)
            => _dimmer.SetActive(mustHidden);

        public void SetVisible(bool isVisible)
            => gameObject.SetActive(isVisible);
    }
}