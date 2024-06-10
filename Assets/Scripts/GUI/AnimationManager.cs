using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Dev.Scripts.System;
using GameStates;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

#if UNITY_ADS
using UnityEngine.Advertisements;
#endif


namespace Dev.Scripts.GUI
{
    public class AnimationManager : MonoBehaviour
{
    public bool playOnEnable = true;
    private bool _waitForPickupFriends;

    private bool _waitForAksFriends;
    private Dictionary<string, string> _parameters;

    void OnEnable()
    {
        if (playOnEnable)
        {
            SoundBase.Instance.GetComponent<AudioSource>().PlayOneShot(SoundBase.Instance.swish[0]);
            
        }
        if (name == "MenuPlay")
        {
            for (int i = 1; i <= 3; i++)
            {
                transform.Find("Image/Stars").Find("Star" + i).gameObject.SetActive(false);
            }

            int stars = new PlayerPrefsMapProgressManager().LoadLevelStarsCount(GameManager.Instance.currentLevel);

            if (stars > 0 && stars <= 3)
            {
                for (int i = 1; i <= stars; i++)
                {
                    transform.Find("Image/Stars").Find("Star" + i).gameObject.SetActive(true);
                }

            }
            else
            {
                for (int i = 1; i <= 3; i++)
                {
                    transform.Find("Image/Stars").Find("Star" + i).gameObject.SetActive(false);
                }

            }
        }

        if (name == "PrePlay")
        {
            // GameObject
        }
        if (name == "PreFailed")
        {
            SoundBase.Instance.GetComponent<AudioSource>().PlayOneShot(SoundBase.Instance.gameOver[0]);
            transform.Find("Video").gameObject.SetActive(true);
            transform.Find("Buy").GetComponent<Button>().interactable = true;

            GetComponent<Animation>().Play();
        }

        if (name == "Settings" || name == "MenuPause")
        {
            if (PlayerPrefs.GetInt(PlayerPrefsKeys.Sound) == 0)
            {
                transform.Find("Image/Sound/SoundOff").gameObject.SetActive(true);
                transform.Find("Image/Sound/SoundOn").gameObject.SetActive(false);
            }
            else
            {
                transform.Find("Image/Sound/SoundOff").gameObject.SetActive(false);
                transform.Find("Image/Sound/SoundOn").gameObject.SetActive(true);
            }


            if (PlayerPrefs.GetInt(PlayerPrefsKeys.Music) == 0)
            {
                transform.Find("Image/Music/MusicOff").gameObject.SetActive(true);
                transform.Find("Image/Music/MusicOn").gameObject.SetActive(false);
            }
            else
            {
                transform.Find("Image/Music/MusicOff").gameObject.SetActive(false);
                transform.Find("Image/Music/MusicOn").gameObject.SetActive(true);
            }
        }

        if (name == "GemsShop")
        {
            for (int i = 1; i <= 4; i++)
            {
                transform.Find("Image/Pack" + i + "/Count").GetComponent<Text>().text = "" + GameManager.Instance.gemsProducts[i - 1].count;
                transform.Find("Image/Pack" + i + "/Buy/Price").GetComponent<Text>().text = "$" + GameManager.Instance.gemsProducts[i - 1].price;
            }
        }
        if (name == "MenuComplete")
        {
            for (int i = 1; i <= 3; i++)
            {
                transform.Find("Image/Stars").Find("Star" + i).gameObject.SetActive(false);
            }

        }

        var videoAdsButton = transform.Find("Image/Video");
        if (videoAdsButton == null) videoAdsButton = transform.Find("Video");
        if (videoAdsButton != null )
        {
            var videoButton = videoAdsButton == null ? transform.Find("Video") : videoAdsButton;
            if (videoButton == null) return;
            if (videoButton.GetComponent<RewardedButton>() == null)
            {
                RewardedButton b = videoButton.gameObject.AddComponent<RewardedButton>();
                b.type = GetReward();
                b.SetEnabled();
            }
#if UNITY_ADS
            InitScript.Instance.rewardedVideoZone = "rewardedVideo";

            if (!InitScript.Instance.enableUnityAds || !InitScript.Instance.GetRewardedUnityAdsReady())
            {
                videoButton.gameObject.SetActive(false);
            }
#elif GOOGLE_MOBILE_ADS
			bool stillShow = true;
#if UNITY_ADS
        stillShow = !InitScript.Instance.GetRewardedUnityAdsReady ();
#endif
			if (!InitScript.Instance.enableGoogleMobileAds || !RewAdmobManager.THIS.IsRewardedAdIsLoaded() || !stillShow)
				videoAdsButton.gameObject.SetActive(false);
#else
			videoAdsButton.gameObject.SetActive(false);
#endif
        }

    }

    void Update()
    {

        if (Input.GetKeyUp(KeyCode.Escape))
        {
            if (name == "MenuPlay" || name == "Settings" || name == "BoostInfo" || name == "GemsShop" || name == "LiveShop" || name == "BoostShop" || name == "Reward")
                CloseMenu();
        }
    }

    public void ShowAds()
    {
        InitScript.Instance.currentReward = GetReward();
        InitScript.Instance.ShowRewardedAds();
        CloseMenu();
    }

    private RewardedAdsType GetReward()
    {
        switch (name)
        {
            case "GemsShop":
                return RewardedAdsType.GetGems;
            case "LiveShop":
                return RewardedAdsType.GetLifes;
            case "PreFailed":
                return RewardedAdsType.GetGoOn;
        }
        return RewardedAdsType.GetGoOn;
    }
    
    void OnDisable()
    {
        if (transform.Find("Image/Video") != null)
        {
            transform.Find("Image/Video").gameObject.SetActive(true);
        }
    }

    public void OnFinished()
    {
        if (name == "MenuComplete")
        {
            StartCoroutine(MenuComplete());
            StartCoroutine(MenuCompleteScoring());
        }
        if (name == "MenuPlay")
        {
            transform.Find("Image/Boost1").GetComponent<BoostIcon>().InitBoost();
            transform.Find("Image/Boost2").GetComponent<BoostIcon>().InitBoost();
            transform.Find("Image/Boost3").GetComponent<BoostIcon>().InitBoost();

        }
        if (name == "MenuPause")
        {
            if (GameManager.Instance.GetState<Playing>())
                GameManager.Instance.ChangeState<PauseState>();
        }

        if (name == "PrePlay")
        {
            CloseMenu();
            GameManager.Instance.ChangeState<WaitForPopup>();

        }
        if (name == "PreFailed")
        {
            if (GameManager.Instance.limit <= 0)
                GameManager.Instance.ChangeState<GameOver>();
            transform.Find("Video").gameObject.SetActive(false);

            CloseMenu();

        }

        if (name.Contains("gratzWord"))
            gameObject.SetActive(false);
        if (name == "NoMoreMatches")
            gameObject.SetActive(false);
        if (name == "CompleteLabel")
            gameObject.SetActive(false);

    }
    
    public void WaitForGiveUp()
    {
        if (name == "PreFailed")
        {
            GetComponent<Animation>()["bannerFailed"].speed = 0;
#if UNITY_ADS

            if (InitScript.Instance.enableUnityAds)
            {
                if (InitScript.Instance.GetRewardedUnityAdsReady())
                {
                    transform.Find("Video").gameObject.SetActive(true);
                }
            }
#elif GOOGLE_MOBILE_ADS
			bool stillShow = true;
#if UNITY_ADS
        stillShow = !InitScript.Instance.GetRewardedUnityAdsReady ();
#endif
			if (InitScript.Instance.enableGoogleMobileAds && stillShow)
			{
				if (RewAdmobManager.THIS.IsRewardedAdIsLoaded())
				{
					transform.Find("Video").gameObject.SetActive(true);
				}
			}

#endif
        }
    }

    IEnumerator MenuComplete()
    {
        for (int i = 1; i <= GameManager.Instance.stars; i++)
        {
            transform.Find("Image/Stars").Find("Star" + i).gameObject.SetActive(true);
            SoundBase.Instance.GetComponent<AudioSource>().PlayOneShot(SoundBase.Instance.star[i - 1]);
            yield return new WaitForSeconds(0.5f);
        }
    }

    IEnumerator MenuCompleteScoring()
    {
        var scores = transform.Find("Image/ScoreBanner").Find("Score").GetComponent<TextMeshProUGUI>();
        for (int i = 0; i <= GameManager.Score; i += 500)
        {
            scores.text = "" + i;
            // SoundBase.Instance.audio.PlayOneShot( SoundBase.Instance.scoring );
            yield return new WaitForSeconds(0.00001f);
        }
        scores.text = "" + GameManager.Score;
    }

    public void Info()
    {
        GameObject.Find("CanvasGlobal").transform.Find("Tutorial").gameObject.SetActive(true);
        CloseMenu();
    }



    public void PlaySoundButton()
    {
        SoundBase.Instance.GetComponent<AudioSource>().PlayOneShot(SoundBase.Instance.click);

    }

    public IEnumerator Close()
    {
        yield return new WaitForSeconds(0.5f);
    }

    public void CloseMenu()
    {
        SoundBase.Instance.GetComponent<AudioSource>().PlayOneShot(SoundBase.Instance.click);
        if (gameObject.name == "MenuPreGameOver")
        {
            ShowGameOver();
        }
        if (gameObject.name == "MenuComplete")
        {
            GameManager.Instance.ChangeState<Map>();
            PlayerPrefs.SetInt(PlayerPrefsKeys.OpenLevel, GameManager.Instance.currentLevel + 1);
            GameManager.Instance.LoadLevel();
            if (LevelsMap.Instance.GetMapLevels().Count >= GameManager.Instance.currentLevel)
                GameObject.Find("CanvasGlobal").transform.Find("MenuPlay").gameObject.SetActive(true);
        }
        if (gameObject.name == "MenuFailed")
        {
            GameManager.Instance.ChangeState<Map>();
        }

        if (SceneManager.GetActiveScene().name == "Game")
        {
            if (GameManager.Instance.GetState<PauseState>())
            {
                GameManager.Instance.ChangeState<WaitAfterClose>();
            }
        }
        SoundBase.Instance.GetComponent<AudioSource>().PlayOneShot(SoundBase.Instance.swish[1]);

        gameObject.SetActive(false);
    }

    public void SwishSound()
    {
        SoundBase.Instance.GetComponent<AudioSource>().PlayOneShot(SoundBase.Instance.swish[1]);

    }

    public void ShowInfo()
    {
        GameObject.Find("CanvasGlobal").transform.Find("BoostInfo").gameObject.SetActive(true);

    }

    public void Play()
    {
        SoundBase.Instance.GetComponent<AudioSource>().PlayOneShot(SoundBase.Instance.click);
        if (gameObject.name == "MenuPreGameOver")
        {
            if (InitScript.Gems >= 12)
            {
                InitScript.Instance.SpendGems(12);
                GameManager.Instance.ChangeState<WaitAfterClose>();
                gameObject.SetActive(false);

            }
            else
            {
                BuyGems();
            }
        }
        else if (gameObject.name == "MenuFailed")
        {
            GameManager.Instance.ChangeState<Map>();
        }
        else if (gameObject.name == "MenuPlay")
        {
            if (InitScript.Lifes > 0)
            {
                InitScript.Instance.SpendLife(1);
                GameManager.Instance.ChangeState<PrepareGame>();
                CloseMenu();
            }
            else
            {
                BuyLifeShop();
            }

        }
        else if (gameObject.name == "MenuPause")
        {
            CloseMenu();
            GameManager.Instance.ChangeState<Playing>();
        }
    }

    public void PlayTutorial()
    {
        SoundBase.Instance.GetComponent<AudioSource>().PlayOneShot(SoundBase.Instance.click);
        GameManager.Instance.ChangeState<Playing>();
    }

    public void BackToMap()
    {
        Time.timeScale = 1;
        GameManager.Instance.ChangeState<GameOver>();
        CloseMenu();
    }

    public void Next()
    {
        SoundBase.Instance.GetComponent<AudioSource>().PlayOneShot(SoundBase.Instance.click);
        CloseMenu();
    }

    public void Again()
    {
        GameObject gm = new GameObject();
        gm.AddComponent<RestartLevel>();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void BuyGems()
    {

        SoundBase.Instance.GetComponent<AudioSource>().PlayOneShot(SoundBase.Instance.click);
        GameObject.Find("CanvasGlobal").transform.Find("GemsShop").gameObject.SetActive(true);
    }

    public void Buy(GameObject pack)
    {
        SoundBase.Instance.GetComponent<AudioSource>().PlayOneShot(SoundBase.Instance.click);
        if (pack.name == "Pack1")
        {
            InitScript.WaitedPurchaseGems = int.Parse(pack.transform.Find("Count").GetComponent<Text>().text.Replace("x ", ""));
#if UNITY_WEBPLAYER || UNITY_WEBGL
            InitScript.Instance.PurchaseSucceded();
            CloseMenu();
            return;
#endif
#if UNITY_INAPPS
            UnityInAppsIntegration.THIS.BuyProductID(LevelManager.THIS.InAppIDs[0]);
#else
            Debug.LogError("Unity-inapps not enable. More info: https://docs.google.com/document/d/1HeN8JtQczTVetkMnd8rpSZp_TZZkEA7_kan7vvvsMw0/edit#heading=h.60xg5ccbex9m");//2.1.6
#endif
        }

        if (pack.name == "Pack2")
        {
            InitScript.WaitedPurchaseGems = int.Parse(pack.transform.Find("Count").GetComponent<Text>().text.Replace("x ", ""));
#if UNITY_WEBPLAYER || UNITY_WEBGL
            InitScript.Instance.PurchaseSucceded();
            CloseMenu();
            return;
#endif
#if UNITY_INAPPS
            UnityInAppsIntegration.THIS.BuyProductID(LevelManager.THIS.InAppIDs[1]);
#else
            Debug.LogError("Unity-inapps not enable. More info: https://docs.google.com/document/d/1HeN8JtQczTVetkMnd8rpSZp_TZZkEA7_kan7vvvsMw0/edit#heading=h.60xg5ccbex9m");//2.1.6
#endif
        }
        if (pack.name == "Pack3")
        {
            InitScript.WaitedPurchaseGems = int.Parse(pack.transform.Find("Count").GetComponent<Text>().text.Replace("x ", ""));
#if UNITY_WEBPLAYER || UNITY_WEBGL
            InitScript.Instance.PurchaseSucceded();
            CloseMenu();
            return;
#endif
#if UNITY_INAPPS
            UnityInAppsIntegration.THIS.BuyProductID(LevelManager.THIS.InAppIDs[2]);
#else
            Debug.LogError("Unity-inapps not enable. More info: https://docs.google.com/document/d/1HeN8JtQczTVetkMnd8rpSZp_TZZkEA7_kan7vvvsMw0/edit#heading=h.60xg5ccbex9m");//2.1.6
#endif
        }
        if (pack.name == "Pack4")
        {
            InitScript.WaitedPurchaseGems = int.Parse(pack.transform.Find("Count").GetComponent<Text>().text.Replace("x ", ""));
#if UNITY_WEBPLAYER || UNITY_WEBGL
            InitScript.Instance.PurchaseSucceded();
            CloseMenu();
            return;
#endif
#if UNITY_INAPPS
            UnityInAppsIntegration.THIS.BuyProductID(LevelManager.THIS.InAppIDs[3]);
#else
            Debug.LogError("Unity-inapps not enable. More info: https://docs.google.com/document/d/1HeN8JtQczTVetkMnd8rpSZp_TZZkEA7_kan7vvvsMw0/edit#heading=h.60xg5ccbex9m");//2.1.6
#endif
        }
        CloseMenu();

    }

    public void BuyLifeShop()
    {

        SoundBase.Instance.GetComponent<AudioSource>().PlayOneShot(SoundBase.Instance.click);
        if (InitScript.Lifes < InitScript.Instance.capOfLife)
            GameObject.Find("CanvasGlobal").transform.Find("LiveShop").gameObject.SetActive(true);

    }

    public void BuyLife(GameObject button)
    {
        SoundBase.Instance.GetComponent<AudioSource>().PlayOneShot(SoundBase.Instance.click);
        if (InitScript.Gems >= int.Parse(button.transform.Find("Price").GetComponent<Text>().text))
        {
            InitScript.Instance.SpendGems(int.Parse(button.transform.Find("Price").GetComponent<Text>().text));
            InitScript.Instance.RestoreLifes();
            CloseMenu();
        }
        else
        {
            GameObject.Find("CanvasGlobal").transform.Find("GemsShop").gameObject.SetActive(true);
        }

    }

    public void BuyFailed(GameObject button)
    {
        if (GetComponent<Animation>()["bannerFailed"].speed == 0)
        {
            if (InitScript.Gems >= int.Parse(button.transform.Find("Price").GetComponent<Text>().text))
            {
                InitScript.Instance.SpendGems(int.Parse(button.transform.Find("Price").GetComponent<Text>().text));
                button.GetComponent<Button>().interactable = false;
                GoOnFailed();
            }
            else
            {
                GameObject.Find("CanvasGlobal").transform.Find("GemsShop").gameObject.SetActive(true);
            }
        }
    }

    public void GoOnFailed()
    {
        if (GameManager.Instance.limitType == LIMIT.MOVES)
            GameManager.Instance.limit += GameManager.Instance.extraFailedMoves;
        else
            GameManager.Instance.limit += GameManager.Instance.extraFailedSecs;
        GetComponent<Animation>()["bannerFailed"].speed = 1;
        GameManager.Instance.ChangeState<Playing>();
    }

    public void GiveUp()
    {
        GetComponent<Animation>()["bannerFailed"].speed = 1;
    }

    void ShowGameOver()
    {
        SoundBase.Instance.GetComponent<AudioSource>().PlayOneShot(SoundBase.Instance.gameOver[1]);

        GameObject.Find("Canvas").transform.Find("MenuGameOver").gameObject.SetActive(true);
        gameObject.SetActive(false);

    }

    #region boosts

    public void BuyBoost(BoostType boostType, int price, int count)
    {
        SoundBase.Instance.GetComponent<AudioSource>().PlayOneShot(SoundBase.Instance.click);
        if (InitScript.Gems >= price)
        {
            InitScript.Instance.SpendGems(price);
            InitScript.Instance.BuyBoost(boostType, price, count);
            CloseMenu();
        }
        else
        {
            BuyGems();
        }
    }

    #endregion

    public void SoundOff(GameObject Off)
    {
        if (!Off.activeSelf)
        {
            SoundBase.Instance.GetComponent<AudioSource>().volume = 0;
            InitScript.Sound = false;

            Off.SetActive(true);
        }
        else
        {
            SoundBase.Instance.GetComponent<AudioSource>().volume = 1;
            InitScript.Sound = true;

            Off.SetActive(false);

        }
        PlayerPrefs.SetInt(PlayerPrefsKeys.Sound, (int)SoundBase.Instance.GetComponent<AudioSource>().volume);
        PlayerPrefs.Save();

    }

    public void MusicOff(GameObject Off)
    {
        if (!Off.activeSelf)
        {
            GameObject.Find("Music").GetComponent<AudioSource>().volume = 0;
            InitScript.Music = false;

            Off.SetActive(true);
        }
        else
        {
            GameObject.Find("Music").GetComponent<AudioSource>().volume = 1;
            InitScript.Music = true;

            Off.SetActive(false);

        }
        PlayerPrefs.SetInt(PlayerPrefsKeys.Music, (int)GameObject.Find("Music").GetComponent<AudioSource>().volume);
        PlayerPrefs.Save();

    }

}
}