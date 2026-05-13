using UnityEngine;
using Zenject;

namespace Infrastructure.Input
{
    public class InputService
    {
        public Vector2 MousePosition => UserInputControls.BattleSelection.PointerPosition.ReadValue<Vector2>();
        public InputControls UserInputControls { get; }
        
        [Inject]
        public InputService()
        {
            UserInputControls = new InputControls();
        }

        public void OnDispose()
        {
            UserInputControls.Disable();
            UserInputControls?.Dispose();
        }
    }
}