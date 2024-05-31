using System.Collections;
using GameStates;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Dev.Scripts.System
{

    public class RestartLevel : MonoBehaviour
    {
        private void Awake()
        {
            DontDestroyOnLoad(this);
        }

        void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            StartCoroutine(WaitForLoad(scene));
        }

        IEnumerator WaitForLoad(Scene scene)
        {
            yield return new WaitUntil(()=>LevelManager.Instance != null);
            if(scene.name == "game")
            {
                Debug.Log("restart");

                StartGame();
                Destroy(gameObject);
            }
        }

        void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        
        void OnDisable()
        {
            Debug.Log("OnDisable");
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
        
        public void StartGame()
        {
            if (InitScript.lifes > 0)
            {
                InitScript.Instance.SpendLife(1);
                GameManager.Instance.ChangeState<PrepareGame>();
            }
            else
            {
                BuyLifeShop();
            }

        }
        
        public void BuyLifeShop()
        {

            SoundBase.Instance.GetComponent<AudioSource>().PlayOneShot(SoundBase.Instance.click);
            if (InitScript.lifes < InitScript.Instance.CapOfLife)
                GameObject.Find("CanvasGlobal").transform.Find("LiveShop").gameObject.SetActive(true);

        }
    }

}