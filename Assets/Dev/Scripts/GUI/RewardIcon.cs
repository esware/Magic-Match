using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace Dev.Scripts.GUI
{

    public class RewardIcon : MonoBehaviour
    {
        public Sprite[] sprites;
        public Image icon;
        
        public void SetIconSprite(int i)
        {
            icon.sprite = sprites[i];
        }
    }
}