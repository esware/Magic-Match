using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace Dev.Scripts.GUI
{
    public class LifeShop : MonoBehaviour
    {
        public int costIfRefill = 12;

        public int CostIfRefill { get; set; }

        // Use this for initialization
        void OnEnable ()
        {
            transform.Find ("Image/BuyLife/Price").GetComponent<Text> ().text = "" + costIfRefill;
            if (!LevelManager.Instance.enableInApps)
                transform.Find ("Image/BuyLife").gameObject.SetActive (false);
		
        }
    }
}