using System.Collections.Generic;
using CustomTypes;
using CustomTypes.Enums.Team;
using Dices;
using UnityEngine;

namespace UI.Battle
{
    public class BattleDicesUIView : MonoBehaviour
    {
        [SerializeField] private Transform _playerDicesContent;
        [SerializeField] private Transform _enemyDicesContent;
        
        public Transform PlayerDicesContent => _playerDicesContent;
        public Transform EnemyDicesContent => _enemyDicesContent;

        private readonly Dictionary<TeamType, Dictionary<int, DiceUIView>> _dices = new();
        
        public void ShowTeamDices((TeamType Team, List<DiceData> Dices) teamDices)
        {
            if (teamDices.Dices == null)
                return;
            
            foreach (var diceData in teamDices.Dices)
                _dices[teamDices.Team][diceData.Unit].UpdateView(diceData);
        }
 
        public void AddDiceUI(TeamType team, int unit, DiceUIView dice)
        {
            if (!_dices.ContainsKey(team))
                _dices.Add(team, new Dictionary<int, DiceUIView>());
            
            _dices[team][unit] = dice;
        }
    }
}