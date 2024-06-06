
using UnityEngine;
using System.Collections;
using UnityEngine.UI;


namespace Dev.Scripts.GUI
{
    public class MovesLabel : MonoBehaviour 
    {
        public Sprite[] sprites;
        void OnEnable () {
            GameEvents.OnLevelLoaded += Reset;
        }

        void OnDisable () {
            GameEvents.OnLevelLoaded -= Reset;
        }


        void Reset () 
        {
            if (GameManager.Instance != null) {
                if (GameManager.Instance.limitType == LIMIT.MOVES)
                    GetComponent<Image> ().sprite = sprites [0];
                else
                    GetComponent<Image> ().sprite = sprites [1];
            }

        }
    }
}