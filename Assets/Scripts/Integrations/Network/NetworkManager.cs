using System;
using UnityEngine;
using System.Collections.Generic;
using Dev.Scripts.Integrations;
using Dev.Scripts.Integrations.Network;
using Firebase;
using Firebase.Auth;
using Firebase.Extensions;
using Integrations.Network.Firebase;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NetworkManager : MonoBehaviour
{
    public delegate void NetworkEvents();

    public static event NetworkEvents OnLoginEvent;
    public static event NetworkEvents OnLogoutEvent;

    private static NetworkManager _instance;

    public static NetworkManager Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject networkManager = new GameObject();
                _instance = networkManager.AddComponent<NetworkManager>();
                networkManager.name = typeof(NetworkManager).ToString() + " (NetworkManager)";
                DontDestroyOnLoad(networkManager);
            }
            return _instance;
        }
    }
    public static ICurrencyManager currencyManager;
    public static IDataManager dataManager;

    [HideInInspector]
    private static string _userID;

    public static string UserID
    {
        get => _userID;
        set
        {
            if (value != PlayerPrefs.GetString("UserID") && PlayerPrefs.GetString("UserID") != "" && _userID != "" && _userID != null)
            {
                PlayerPrefs.DeleteAll();
                LevelsMap._instance.Reset();
            }

            _userID = value;
            PlayerPrefs.SetString("UserID", _userID);
            PlayerPrefs.Save();
        }
    }

    public bool IsLoggedIn
    {
        get => _isLoggedIn;
        set
        {
            _isLoggedIn = value;
            if (value && OnLoginEvent != null)
                OnLoginEvent();
            else if (!value && OnLogoutEvent != null)
                OnLogoutEvent();
        }
    }
    private bool _isLoggedIn;

    void Start()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            FirebaseApp app = FirebaseApp.DefaultInstance;
            currencyManager = new FirebaseCurrencyManager(UserID);
            dataManager = new FirebaseDataManager(UserID);
        });
        OnLoginEvent += OnUserLoggedIn;
    }
    
    private void OnUserLoggedIn()
    {
        Debug.Log("User logged in, initializing game...");
        LoadingManager.Instance.ShowLoading();
    }

    public void RegisterWithFirebase(string email,string username, string password, TextMeshProUGUI statusText)
    {
        FirebaseManager.Instance.Register(email, username,password, statusText);
        LoginWithFirebase(email,password,statusText);
    }
    
    

    public void LoginWithFirebase(string email, string password, TextMeshProUGUI statusText)
    {
        FirebaseManager.Instance.Login(email, password, statusText);
    }

    public void LoginWithGoogle(string idToken, TextMeshProUGUI statusText)
    {
        FirebaseManager.Instance.LoginWithGoogle(idToken, statusText);
    }

    public void LoginWithPlayFab(string email, string password, TextMeshProUGUI statusText)
    {
        PlayFabManager.Instance.Login(email, password, statusText);
    }

    public void Logout()
    {
        FirebaseManager.Instance.Logout();
    }

    public void UpdatePlayFabUserData(string key, string value)
    {
        PlayFabManager.Instance.UpdateUserData(key, value);
    }

    public void IncreaseCurrency(int amount)
    {
        currencyManager.IncBalance(amount);
    }

    public void DecreaseCurrency(int amount)
    {
        currencyManager.DecBalance(amount);
    }

    public void SetCurrency(int newBalance)
    {
        currencyManager.SetBalance(newBalance);
    }

    public void GetCurrencyBalance(Action<int> callback)
    {
        currencyManager.GetBalance(callback);
    }
}
