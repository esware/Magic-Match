using UnityEngine;
using System.Collections;
using GameStates;
using UnityEngine.SceneManagement;


namespace Dev.Scripts.GUI
{
    public class GUIEvents : MonoBehaviour
    {
        public void Settings () 
        {
            SoundBase.Instance.GetComponent<AudioSource> ().PlayOneShot (SoundBase.Instance.click);

            GameObject.Find ("CanvasGlobal").transform.Find ("Settings").gameObject.SetActive (true);

        }

        public void Play () {
            SoundBase.Instance.GetComponent<AudioSource> ().PlayOneShot (SoundBase.Instance.click);

            transform.Find ("Loading").gameObject.SetActive (true);
            SceneManager.LoadScene ("Game");
        }
        public void Pause () {
            SoundBase.Instance.GetComponent<AudioSource> ().PlayOneShot (SoundBase.Instance.click);

            if ( GameManager.Instance.GetState<Playing>())
                GameObject.Find ("CanvasGlobal").transform.Find ("MenuPause").gameObject.SetActive (true);

        }
        public void LogoutPopup()
        {
            SoundBase.Instance.GetComponent<AudioSource> ().PlayOneShot (SoundBase.Instance.click);

            GameObject.Find ("CanvasGlobal").transform.Find ("MenuLogout").gameObject.SetActive (true);
        }
        public void LoginPopup()
        {
            SoundBase.Instance.GetComponent<AudioSource> ().PlayOneShot (SoundBase.Instance.click);

            GameObject.Find ("CanvasGlobal").transform.Find ("Popup_Login").gameObject.SetActive (true);
        }
        public void RegisterPopup()
        {
            SoundBase.Instance.GetComponent<AudioSource> ().PlayOneShot (SoundBase.Instance.click);

            GameObject.Find ("CanvasGlobal").transform.Find ("Popup_Signup").gameObject.SetActive (true);
        }

    }
}