using GameStates;
using UnityEngine;

namespace Dev.Scripts.GUI
{
    public class GameFieldAnim : MonoBehaviour {

        void EndAnimGamField()
        {
             GameManager.Instance.ChangeState<Playing>();
        }
    }
}