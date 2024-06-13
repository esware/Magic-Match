using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingManager : MonoBehaviour
{
    public static LoadingManager Instance;
    public GameObject loadingCanvas;
    public Image loadingBar;
    public TextMeshProUGUI loadingBarText;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private IEnumerator LoadingScreen()
    {
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(1);
        asyncOperation.allowSceneActivation = false;

        var t = 0f;
        var duration = 5f;
        
        loadingBar.fillAmount = 0;
        
        while (!asyncOperation.isDone)
        {
            
            float progress = Mathf.Clamp01(t / duration);
            loadingBar.fillAmount = Mathf.Lerp(0, 1, progress);
            loadingBarText.text = "%" + (loadingBar.fillAmount * 100).ToString("F0");
            t += Time.deltaTime;
            
            if (asyncOperation.progress >= 0.9f && progress >= 1)
            {
                asyncOperation.allowSceneActivation = true;
            }
            yield return null;
        }
    }


    public void ShowLoading()
    {
        loadingCanvas.SetActive(true);
        StartCoroutine(LoadingScreen());
    }

    public void HideLoading()
    {
        loadingCanvas.SetActive(false);
    }
}

