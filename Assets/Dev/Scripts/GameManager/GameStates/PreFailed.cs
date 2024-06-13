using UnityEngine;

namespace GameStates
{
    public class PreFailed:GameState
    {
        public override void EnterState()
        {
            GameObject.Find("CanvasGlobal").transform.Find("PreFailed").gameObject.SetActive(true);
        }

        public override void UpdateState()
        {
           
        }

        public override void ExitState()
        {
            
        }
    }
}