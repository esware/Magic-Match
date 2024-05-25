using UnityEngine;

namespace Dev.Scripts.GUI
{
    public class GameFieldAnim : MonoBehaviour {

        void EndAnimGamField()
        {
            LevelManager.Instance.GameStatus = GameState.Playing;
        }
    }
}