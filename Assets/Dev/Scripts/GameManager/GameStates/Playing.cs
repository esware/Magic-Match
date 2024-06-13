using UnityEngine;

namespace GameStates
{
    public class Playing:GameState
    {
        public override void EnterState()
        {
            Time.timeScale = 1;
            StartCoroutine(AI.THIS.CheckPossibleCombines());
        }

        public override void UpdateState()
        {
            
        }

        public override void ExitState()
        {
            
        }
    }
}