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


        public void SetTeamRolls(TeamType team, int rollsLeft)
        {
            var textField = team switch
            {
                TeamType.Player => _playerRollsCount,
                TeamType.Enemy => _enemyRollsCount,
                _ => null,
            };
            
            if (textField == null)
            {
                LogService.LogDebug(DebugType.Error, $"No rolls field for team {team}");
                return;
            }
            
            textField.text = string.Format(TextKeys.ROLLS_LEFT, rollsLeft);
        }

        public void SetRollButtonLabel(string label)
            => _rerollButton.Text = label;
    }
}