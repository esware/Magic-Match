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
            throw new System.NotImplementedException();
        }

        public override void ExitState()
        {
            throw new System.NotImplementedException();
        }
    }
}