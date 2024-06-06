using UnityEngine;

namespace GameStates
{
    public class WaitForPopup:GameState
    {
        public override void EnterState()
        {
            GameManager.Instance.InitLevel();
            //GameEvents.OnLevelLoaded?.Invoke();
            
            Debug.Log("WaitForPopup");
        }

        public override void UpdateState()
        {
            
        }

        public override void ExitState()
        {
            
        }
    }
}