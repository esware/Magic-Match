using UnityEngine;

namespace Dev.Scripts.GUI
{

    public class ClosePopup : MonoBehaviour
    {
        public GameObject popupToClose;
        

        public void Close()
        {
            if (popupToClose != null)
            {
                popupToClose.SetActive(false);
            }
        }
    }

}