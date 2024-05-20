using UnityEngine;

namespace Dev.Scripts.GUI
{
    public class GameFieldAnim : MonoBehaviour {

        void EndAnimGamField()
        {
            LevelManager.THIS.gameStatus = GameState.Playing;
        }
    }
}