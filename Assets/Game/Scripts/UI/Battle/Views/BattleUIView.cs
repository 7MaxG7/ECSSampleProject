using CustomTypes;
using Infrastructure;
using TMPro;
using UnityEngine;

namespace UI.Battle
{
    public class BattleUIView : MonoBehaviour
    {
        [SerializeField] private Transform _rootContent;
        [SerializeField] private GameButtonView _rerollButton;
        [SerializeField] private TMP_Text _playerRollsCount;
        [SerializeField] private TMP_Text _enemyRollsCount;

        public Transform RootContent => _rootContent;
        public GameButtonView RerollButton => _rerollButton;


        public void SetTeamRolls((TeamType Team, int RollsCount) teamRolls)
        {
            switch (teamRolls.Team)
            {
                case TeamType.Player:
                    _playerRollsCount.text = string.Format(TextKeys.ROLLS_LEFT, teamRolls.RollsCount);
                    break;
                case TeamType.Enemy:
                    _enemyRollsCount.text = string.Format(TextKeys.ROLLS_LEFT, teamRolls.RollsCount);
                    break;
            }
        }

        public void SetRollButtonLabel(string label)
            => _rerollButton.Text = label;
    }
}