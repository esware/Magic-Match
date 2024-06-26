using UnityEngine;
using System.Collections;
using System;

using System.Collections.Generic;
using System.Globalization;
using Dev.Scripts.GUI;
using Dev.Scripts.System;
#if UNITY_ADS
using UnityEngine.Advertisements;
#endif

#if GOOGLE_MOBILE_ADS
using GoogleMobileAds.Api;
#endif
using UnityEngine.EventSystems;




public enum Target
{
    SCORE,
    COLLECT,
    INGREDIENT,
    BLOCKS
}

public enum LIMIT
{
    MOVES,
    TIME
}

public enum Ingredients
{
    None = 0,
    Ingredient1,
    Ingredient2
}

public enum CollectItems
{
    None = 0,
    Item1,
    Item2,
    Item3,
    Item4,
    Item5,
    Item6
}

public enum RewardedAdsType
{
    GetLifes,
    GetGems,
    GetGoOn
}

public static class PlayerPrefsKeys
{
    public static readonly string RestLifeTimer = "RestLifeTimer";
    public static readonly string Gems = "Gems";
    public static readonly string Lifes = "Lifes";
    public static readonly string DateOfExit = "DateOfExit";
    public static readonly string Launched = "Launched";
    public static readonly string Music = "Music";
    public static readonly string Sound = "Sound";
    public static readonly string OpenLevel = "OpenLevel";
    public static readonly string OpenLevelTest = "OpenLevelTest";
    public static readonly string Score = "Score";
    public static readonly string UserID = "UserID";
}

public class InitScript : MonoBehaviour
{
    public static InitScript Instance;

    #region Public Variables

    public List<AdEvents> adsEvents = new List<AdEvents>();
    public bool enableUnityAds;
    public bool enableGoogleMobileAds;
    public bool enableChartboostAds;
    public string rewardedVideoZone;
    public string nonRewardedVideoZone;
    public int ShowAdmobAdsEveryLevel;
    public int dailyRewardedFrequency;
    public RewardedAdsTime dailyRewardedFrequencyTime;
    public int[] dailyRewardedShown;
    public DateTime[] dailyRewardedShownDate;
    
/*
    public string admobUIDAndroid;
    public string admobUIDIOS;
    public string admobRewardedUIDAndroid;
    public string admobRewardedUIDIOS;
    public bool LoginEnable;*/
    
    public int rewardedGems = 5;
    public bool losingLifeEveryGame;

    #region static variables

    public static Sprite ProfilePic;
    public static bool Sound = false;
    public static bool Music = false;
    
    public static float RestLifeTimer;
    public static string DateOfExit;
    public static DateTime Today;
    
    public static string TimeForReps;
    public static int Gems;
    public static int WaitedPurchaseGems;
    public static int OpenLevel { get; set; }
    public static DateTime DateOfRestLife { get; set; }
    
    #endregion
    
    
    public RewardedAdsType currentReward;
    
    public int capOfLife = 5;
    public float totalTimeForRestLifeHours = 0;
    public float totalTimeForRestLifeMin = 15;
    public float totalTimeForRestLifeSec = 60;
    public int firstGems = 20;
    
    #endregion
    

    #region Private Variables
    
    private static int _lifes;
    
    private int _boostExtraMoves;
    private int _boostPackages;
    private int _boostStripes;
    private int _boostExtraTime;
    private int _boostBomb;
    private int _boostColorfulBomb;
    private int _boostHand;
    private int _boostRandomColor;
    private bool _leftControl;
    private bool _adsReady;
    
#if GOOGLE_MOBILE_ADS
    private InterstitialAd interstitial;
    private AdRequest requestAdmob;
#endif
    
    private string _lastResponse = string.Empty;
    private string _status = "Ready";
    public string admobUIDAndroid;
    public string admobUIDIOS;
    public string admobRewardedUIDAndroid;
    public string admobRewardedUIDIOS;

    #endregion
    
    #region Properties

    public static int Lifes
    {
        get => _lifes;
        set => _lifes = value;
    }
    protected string LastResponse
    {
        get => _lastResponse;

        set => _lastResponse = value;
    }
    protected string Status
    {
        get => _status;

        set => _status = value;
    }

    #endregion

    
    void Awake()
    {
        Application.targetFrameRate = 60;
        Instance = this;
        if (PlayerPrefs.HasKey(PlayerPrefsKeys.RestLifeTimer))
        {
            RestLifeTimer = PlayerPrefs.GetFloat(PlayerPrefsKeys.RestLifeTimer);
        }
        else
        {
            RestLifeTimer = 15 * 60;
        }
        
        DateOfExit = PlayerPrefs.GetString(PlayerPrefsKeys.DateOfExit, "");

        Gems = PlayerPrefs.GetInt(PlayerPrefsKeys.Gems);
        Lifes = PlayerPrefs.GetInt(PlayerPrefsKeys.Lifes);
        {
            dailyRewardedShown = new int[Enum.GetValues(typeof(RewardedAdsType)).Length];
            dailyRewardedShownDate = new DateTime[Enum.GetValues(typeof(RewardedAdsType)).Length];
            for (int i = 0; i < dailyRewardedShown.Length; i++)
            {
                dailyRewardedShown[i] = PlayerPrefs.GetInt(((RewardedAdsType)i).ToString());
                dailyRewardedShownDate[i] = DateTimeManager.GetLastDateTime(((RewardedAdsType)i).ToString());
            }
        }
        if (PlayerPrefs.GetInt(PlayerPrefsKeys.Launched) == 0)
        {
            Lifes = capOfLife;
            Gems = firstGems;
            PlayerPrefs.SetInt(PlayerPrefsKeys.Lifes, Lifes);
            PlayerPrefs.SetInt(PlayerPrefsKeys.Gems, Gems);
            PlayerPrefs.SetInt(PlayerPrefsKeys.Music, 1);
            PlayerPrefs.SetInt(PlayerPrefsKeys.Sound, 1);

            PlayerPrefs.SetInt(PlayerPrefsKeys.Launched, 1);
            PlayerPrefs.Save();
        }

        if (gameObject.GetComponent<AspectCamera>() == null) gameObject.AddComponent<AspectCamera>().map = 
            FindObjectOfType<LevelsMap>().transform.Find("map_background_01").GetComponent<SpriteRenderer>().sprite;

        GameObject.Find("Music").GetComponent<AudioSource>().volume = PlayerPrefs.GetInt("Music");
        SoundBase.Instance.GetComponent<AudioSource>().volume = PlayerPrefs.GetInt("Sound");
/*
#if UNITY_ADS
        enableUnityAds = true;
#else
        enableUnityAds = false;
#endif
        
#if GOOGLE_MOBILE_ADS
        enableGoogleMobileAds = true;
#if UNITY_ANDROID
        MobileAds.Initialize(admobUIDAndroid);//2.1.6
        interstitial = new InterstitialAd(admobUIDAndroid);
#elif UNITY_IOS
        MobileAds.Initialize(admobUIDIOS);
        interstitial = new InterstitialAd(admobUIDIOS);
#else
        MobileAds.Initialize(admobUIDAndroid);//2.1.6
		interstitial = new InterstitialAd (admobUIDAndroid);
#endif
        
        requestAdmob = new AdRequest.Builder().Build();
        interstitial.LoadAd(requestAdmob);
        interstitial.OnAdLoaded += HandleInterstitialLoaded;
        interstitial.OnAdFailedToLoad += HandleInterstitialFailedToLoad;
#else
        enableGoogleMobileAds = false;
#endif
        Transform canvas = GameObject.Find("CanvasGlobal").transform;
        foreach (Transform item in canvas)
        {
            item.gameObject.SetActive(false);
        }*/
    }


    
    #region Ads
    
#if GOOGLE_MOBILE_ADS

    public void HandleInterstitialLoaded(object sender, EventArgs args)
    {
        print("HandleInterstitialLoaded event received.");
    }

    public void HandleInterstitialFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    {
        print("HandleInterstitialFailedToLoad event received with message: " + args.LoadAdError);
    }
#endif
    public bool GetRewardedUnityAdsReady()
    {
#if UNITY_ADS

        rewardedVideoZone = "rewardedVideo";
        if (Advertisement.IsReady(rewardedVideoZone))
        {
            return true;
        }
        else
        {
            rewardedVideoZone = "rewardedVideoZone";
            if (Advertisement.IsReady(rewardedVideoZone))
            {
                return true;
            }
        }
#endif

        return false;
    }
    public void ShowRewardedAds()
    {
#if UNITY_ADS
        Debug.Log("show Unity Rewarded ads video in " + LevelManager.THIS.gameStatus);

        if (GetRewardedUnityAdsReady())
        {
            Advertisement.Show(rewardedVideoZone, new ShowOptions
            {
                resultCallback = result =>
                {
                    if (result == ShowResult.Finished)
                    {
                        CheckRewardedAds();
                    }
                }
            });
        }

#elif GOOGLE_MOBILE_ADS
        bool stillShow = true;
#if UNITY_ADS
        stillShow = !GetRewardedUnityAdsReady ();
#endif
        if(stillShow)
        {
            Debug.Log("show Admob Rewarded ads video in " + "Write state");
           // RewAdmobManager.This.ShowRewardedAd(CheckRewardedAds);
        }
#endif
    }
    
    public bool RewardedReachedLimit(RewardedAdsType type)
    {
        if (dailyRewardedFrequency == 0) return false;
        dailyRewardedShown[(int)type] = PlayerPrefs.GetInt(type.ToString());
        if (!DateTimeManager.IsPeriodPassed(type.ToString())) return true;
        if (dailyRewardedFrequency > 0 && dailyRewardedShown[(int)type] >= dailyRewardedFrequency) return true;
        dailyRewardedShown[(int)type]++;
        PlayerPrefs.SetInt(type.ToString(), dailyRewardedShown[(int)type]);
        if (dailyRewardedShown[(int)type] >= dailyRewardedFrequency) DateTimeManager.SetDateTimeNow(type.ToString());
        PlayerPrefs.Save();

        return false;
    }
    public void CheckAdsEvents(GameState state)
    {
        foreach (AdEvents item in adsEvents)
        {
            if (item.gameEvent == state)
            {
                item.calls++; 
                if (item.calls % item.everyLevel == 0)
                    ShowAdByType(item.adType);
            }
        }
    }

    private void ShowAdByType(AdType adType)
    { 
        if (adType == AdType.AdmobInterstitial && enableGoogleMobileAds)
            ShowAds(false);
        else if (adType == AdType.UnityAdsVideo && enableUnityAds)
            ShowVideo();
        else if (adType == AdType.ChartboostInterstitial && enableChartboostAds)
            ShowAds(true);

    }

    private void ShowVideo()
    { 
#if UNITY_ADS
        Debug.Log("show Unity ads video in " + LevelManager.THIS.gameStatus);

        if (Advertisement.IsReady("video"))
        {
            Advertisement.Show("video");
        }
        else
        {
            if (Advertisement.IsReady("defaultZone"))
            {
                Advertisement.Show("defaultZone");
            }
        }
#endif
    }
    private void ShowAds(bool chartboost = true)
    {
        /*
#if GOOGLE_MOBILE_ADS
            Debug.Log("show Google mobile ads Interstitial in " + LevelManager.THIS.gameStatus);
            if (interstitial.IsLoaded())
            {
                interstitial.Show();
#if UNITY_ANDROID
                interstitial = new InterstitialAd(admobUIDAndroid);
#elif UNITY_IOS
                interstitial = new InterstitialAd(admobUIDIOS);
#else
				interstitial = new InterstitialAd (admobUIDAndroid);
#endif

                // Create an empty ad request.
                requestAdmob = new AdRequest.Builder().Build();
                // Load the interstitial with the request.
                interstitial.LoadAd(requestAdmob);
            }
#endif*/
    }
    
    private void CheckRewardedAds()
    {
        RewardIcon reward = GameObject.Find("CanvasGlobal").transform.Find("Reward").GetComponent<RewardIcon>();
        if (currentReward == RewardedAdsType.GetGems)
        {
            reward.SetIconSprite(0);

            reward.gameObject.SetActive(true);
            AddGems(rewardedGems);
            GameObject.Find("CanvasGlobal").transform.Find("GemsShop").GetComponent<AnimationManager>().CloseMenu();
        }
        else if (currentReward == RewardedAdsType.GetLifes)
        {
            reward.SetIconSprite(1);
            reward.gameObject.SetActive(true);
            RestoreLifes();
            GameObject.Find("CanvasGlobal").transform.Find("LiveShop").GetComponent<AnimationManager>().CloseMenu();
        }
        else if (currentReward == RewardedAdsType.GetGoOn)
        {
            GameObject.Find("CanvasGlobal").transform.Find("PreFailed").GetComponent<AnimationManager>().GoOnFailed();
        }

    }

    #endregion
    
    #region Gems

    public void PurchaseSucceded()
    {
        AddGems(WaitedPurchaseGems);
        WaitedPurchaseGems = 0;
    }
    public void SetGems(int count)
    {
        Gems = count;
        PlayerPrefs.SetInt("Gems", Gems);
        PlayerPrefs.Save();
    }
    public void AddGems(int count)
    {
        Gems += count;
        PlayerPrefs.SetInt(PlayerPrefsKeys.Gems.ToString(), Gems);
        PlayerPrefs.Save();
#if PLAYFAB
        NetworkManager.currencyManager.IncBalance(count);
#endif
        NetworkManager.Instance.IncreaseCurrency(count);

    }
    public void SpendGems(int count)
    {
        SoundBase.Instance.GetComponent<AudioSource>().PlayOneShot(SoundBase.Instance.cash);
        Gems -= count;
        PlayerPrefs.SetInt(PlayerPrefsKeys.Gems.ToString(), Gems);
        PlayerPrefs.Save();
#if PLAYFAB 
        NetworkManager.currencyManager.DecBalance(count);
#endif

    }
    #endregion
    
    #region Life

    public void RestoreLifes()
    {
        _lifes = capOfLife;
        PlayerPrefs.SetInt(PlayerPrefsKeys.Lifes.ToString(), _lifes);
        PlayerPrefs.Save();
    }

    public void AddLife(int count)
    {
        _lifes += count;
        if (_lifes > capOfLife)
            _lifes = capOfLife;
        PlayerPrefs.SetInt(PlayerPrefsKeys.Lifes.ToString(), _lifes);
        PlayerPrefs.Save();
    }

    public int GetLife()
    {
        if (_lifes > capOfLife)
        {
            _lifes = capOfLife;
            PlayerPrefs.SetInt(PlayerPrefsKeys.Lifes.ToString(), _lifes);
            PlayerPrefs.Save();
        }
        return _lifes;
    }
    
    public void SpendLife(int count)
    {
        if (_lifes > 0)
        {
            _lifes -= count;
            PlayerPrefs.SetInt(PlayerPrefsKeys.Lifes, _lifes);
            PlayerPrefs.Save();
        }
    }

    #endregion

    #region Boost
    
    public void BuyBoost(BoostType boostType, int price, int count)
    {
        PlayerPrefs.SetInt("" + boostType, count);
        PlayerPrefs.Save();
#if PLAYFAB 
        NetworkManager.dataManager.SetBoosterData();
#endif
        
    }
    public void SpendBoost(BoostType boostType)
    {
        PlayerPrefs.SetInt("" + boostType, PlayerPrefs.GetInt("" + boostType) - 1);
        PlayerPrefs.Save();
#if PLAYFAB 
        NetworkManager.dataManager.SetBoosterData();
#endif
    }

    #endregion
    
    
    private void OnLevelClicked(int levelNumber)
    {
        if (EventSystem.current.IsPointerOverGameObject(-1))
            return;
        if (!GameObject.Find("CanvasGlobal").transform.Find("MenuPlay").gameObject.activeSelf && 
            !GameObject.Find("CanvasGlobal").transform.Find("GemsShop").gameObject.activeSelf && 
            !GameObject.Find("CanvasGlobal").transform.Find("LiveShop").gameObject.activeSelf && 
            !GameObject.Find("CanvasGlobal").transform.Find("Settings").gameObject.activeSelf)
        {
            PlayerPrefs.SetInt(PlayerPrefsKeys.OpenLevel, levelNumber);
            PlayerPrefs.Save();
            GameEvents.OnMenuPlay?.Invoke();
            GameManager.Instance.LoadLevel();
            OpenLevel = levelNumber;
            GameObject.Find("CanvasGlobal").transform.Find("MenuPlay").gameObject.SetActive(true);
        }
    }
    
    private void OnEnable()
    {
        GameEvents.OnLevelSelected += OnLevelClicked;
    }
    private void OnDisable()
    {
        GameEvents.OnLevelSelected -= OnLevelClicked;
        
        PlayerPrefs.SetInt(PlayerPrefsKeys.Lifes, Lifes);
        PlayerPrefs.SetString(PlayerPrefsKeys.DateOfExit, DateTime.Now.ToString());
        PlayerPrefs.Save();
        
        /*
#if GOOGLE_MOBILE_ADS
        interstitial.OnAdLoaded -= HandleInterstitialLoaded;
        interstitial.OnAdFailedToLoad -= HandleInterstitialFailedToLoad;
#endif
*/

    }
    private void SavePlayerPrefs()
    {
        if (RestLifeTimer > 0)
        {
            PlayerPrefs.SetFloat(PlayerPrefsKeys.RestLifeTimer, 900);
        }
        PlayerPrefs.SetInt(PlayerPrefsKeys.Lifes, Lifes);
        PlayerPrefs.SetString(PlayerPrefsKeys.DateOfExit, DateTime.Now.ToString());
        PlayerPrefs.Save();
    }
    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            SavePlayerPrefs();
        }
    }
    private void OnApplicationQuit()
    {
        SavePlayerPrefs();
    }

}
