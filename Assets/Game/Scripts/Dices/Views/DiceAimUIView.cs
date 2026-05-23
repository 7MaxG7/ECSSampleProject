using CustomTypes;
using TMPro;
using UnityEngine;

namespace Dices
{
    public class DiceAimUIView : MonoBehaviour
    {
        [SerializeField] private TMP_Text _sideText;

        public void SetSide(DiceSide diceSide)
        {
            var text = diceSide.SideType.ToString();
            if (diceSide.IsValued)
                text += $":\n{diceSide.Value}";
            _sideText.text = text;
        }
    }
}