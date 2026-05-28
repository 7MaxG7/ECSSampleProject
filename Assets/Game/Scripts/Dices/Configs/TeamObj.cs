using System;
using CustomTypes;
using UnityEngine;

namespace Dices
{
    [Serializable]
    public class TeamObj
    {
        [SerializeField] private TeamType _team;
        [SerializeField] private GameObject _gObject;

        public TeamType Team => _team;
        public GameObject GObject => _gObject;
    }
}