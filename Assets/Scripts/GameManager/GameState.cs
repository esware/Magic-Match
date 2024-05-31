using UnityEngine;

public abstract class GameState : MonoBehaviour
{
    private GameManager _gameManager;

    public virtual void Initialize(GameManager gameManager)
    {
        _gameManager = gameManager;
    }

    public abstract void EnterState();
    public abstract void UpdateState();
    public abstract void ExitState();
}