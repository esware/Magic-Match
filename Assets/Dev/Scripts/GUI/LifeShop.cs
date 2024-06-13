using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.UI;

namespace Dev.Scripts.GUI
{
    public class LifeShop : MonoBehaviour
    {
        public int costIfRefill = 12;

        public int CostIfRefill { get; set; }
        
        void OnEnable ()
        {
            transform.Find ("Image/BuyLife/Price").GetComponent<TextMeshProUGUI> ().text = "" + costIfRefill;
            if (!GameManager.Instance.enableInApps)
                transform.Find ("Image/BuyLife").gameObject.SetActive (false);
		
        }
    }
}