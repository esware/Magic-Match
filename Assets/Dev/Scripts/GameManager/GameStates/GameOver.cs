using UnityEngine;

namespace GameStates
{
    public class GameOver:GameState
    {
        public override void EnterState()
        {
            GameObject.Find("CanvasGlobal").transform.Find("MenuFailed").gameObject.SetActive(true);
            GameEvents.OnLose?.Invoke();
        }

        public override void UpdateState()
        {
            
        }

        public override void ExitState()
        {
            
        }
    }
}