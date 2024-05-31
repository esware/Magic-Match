using UnityEngine;

namespace GameStates
{
    public class PauseState:GameState
    {
        public override void EnterState()
        {
            Time.timeScale = 0;
        }

        public override void UpdateState()
        {
            
        }

        public override void ExitState()
        {
            
        }
    }
}