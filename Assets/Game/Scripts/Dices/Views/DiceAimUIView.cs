using CustomTypes;
using TMPro;
using UnityEngine;
using Utils;

namespace Dices
{
    public class DiceAimUIView : MonoBehaviour
    {
        [SerializeField] private TMP_Text _sideText;
        [SerializeField] private TeamObj[] _teamDiceBgs;

        public void SetVisible(bool isVisible)
            => gameObject.SetActive(isVisible);

        public void SetTeam(TeamType team)
        {
            foreach (var teamObj in _teamDiceBgs)
                teamObj.GObject.UpdateActive(teamObj.Team == team);
        }

        public void SetSide(DiceSide diceSide)
        {
            var text = diceSide.SideType.ToString();
            if (diceSide.IsValued)
                text += $":\n{diceSide.Value}";
            _sideText.text = text;
        }

        public void SetPosition(Vector2 position)
            => transform.position = position;
    }
}