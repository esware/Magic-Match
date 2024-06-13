using UnityEngine;
using System.Collections;
using UnityEngine.UI;


namespace Dev.Scripts.GUI
{
    public class Background : MonoBehaviour
    {
        public Sprite[] pictures;
        [Header("How often to change background per levels")]
        public int changeBackgoundEveryLevels = 20 ;
        void OnEnable ()
        {
            if (GameManager.Instance != null)
                GetComponent<Image> ().sprite = pictures [Mathf.Clamp( (int)((float)GameManager.Instance.currentLevel / (float)changeBackgoundEveryLevels - 0.01f),0, pictures.Length)];//2.2.2

        }


    }
}