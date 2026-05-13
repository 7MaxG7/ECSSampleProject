using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class GameButtonView : MonoBehaviour
    {
        [SerializeField] private Button _button;
        [SerializeField] private TMP_Text _text;

        public Button.ButtonClickedEvent OnClick => _button.onClick;
        
        public bool Interactable
        {
            set => _button.interactable = value;
        }
        
        public string Text
        {
            get => _text.text;
            set => _text.text = value;
        }
    }
}