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
            _sideText.text = diceSide.ToString();
        }
    }
}