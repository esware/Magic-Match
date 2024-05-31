using System;
using Unity.VisualScripting;
using UnityEngine;

public struct GameEvents
{
    public static Action OnMapState;
    public static Action OnEnterGame;
    public static Action OnLevelLoaded;
    public static Action OnMenuPlay;
    public static Action OnMenuComplete;
    public static Action OnStartPlay;
    public static Action OnWin;
    public static Action OnLose;
}
public class GameManager : MonoBehaviour
{
    private static GameManager _instance;

    public static GameManager Instance
    {
        get
        {
            if (_instance==null)
            {
                var obj = new GameObject("GameManager");
                obj.AddComponent<GameManager>();
                _instance = obj.GetComponent<GameManager>();
            }

            return _instance;
        }
    }
    private GameState _currentState;
    

    public void ChangeState<T>() where T : GameState
    {
        if (_currentState != null)
        {
            _currentState.ExitState();
            Destroy(_currentState);
        }
        _currentState = gameObject.AddComponent<T>();
        _currentState.Initialize(this);
        _currentState.EnterState();
    }
    public T GetState<T>() where T : GameState
    {
        return _currentState as T;
    }

    private void Update()
    {
        if (_currentState != null)
        {
            
        }
    }
}