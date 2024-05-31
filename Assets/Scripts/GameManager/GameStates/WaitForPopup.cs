namespace GameStates
{
    public class WaitForPopup:GameState
    {
        public override void EnterState()
        {
            GameEvents.OnLevelLoaded?.Invoke();
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