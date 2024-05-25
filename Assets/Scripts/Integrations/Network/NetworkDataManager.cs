﻿//#if PLAYFAB 
using System.Collections;
using System.Linq;
using UnityEngine;

#if PLAYFAB
using PlayFab;
using PlayFab.ClientModels;
#endif
using System.Collections.Generic;
using Dev.Scripts.GUI;
using Dev.Scripts.Integrations;

public class NetworkDataManager
{
    private IDataManager _dataManager;
    public static int LatestReachedLevel = 0;
    public static int LevelScoreCurrentRecord = 0;

    public NetworkDataManager()
    {
#if PLAYFAB
		dataManager = new PlayFabDataManager ();
#elif GAMESPARKS
        dataManager = new GamesparksDataManager();
#endif
        NetworkManager.OnLoginEvent += GetPlayerLevel;
        LevelManager.OnEnterGame += GetPlayerScore;
        NetworkManager.OnLogoutEvent += Logout;
        NetworkManager.OnLoginEvent += GetBoosterData;
    }

    public void Logout()
    {
        _dataManager.Logout();
        NetworkManager.OnLoginEvent -= GetPlayerLevel;
        LevelManager.OnEnterGame -= GetPlayerScore;
        NetworkManager.OnLoginEvent -= GetBoosterData;
        NetworkManager.OnLogoutEvent -= Logout;
    }

    #region SCORE

    public void SetPlayerScoreTotal()
    {//2.1.6
        int latestLevel = LevelsMap._instance.GetLastestReachedLevel();
        for (int i = 1; i <= latestLevel; i++)
        {
            SetPlayerScore(i, PlayerPrefs.GetInt("Score" + i, 0));
        }
    }

    public void SetPlayerScore(int level, int score)
    {
        if (!NetworkManager.Instance.IsLoggedIn)
            return;

        if (score <= LevelScoreCurrentRecord)
            return;

        _dataManager.SetPlayerScore(level, score);
    }

    public void GetPlayerScore()
    {
        if (!NetworkManager.Instance.IsLoggedIn)
            return;

        _dataManager.GetPlayerScore((value) =>
        {
            NetworkDataManager.LevelScoreCurrentRecord = value;
            PlayerPrefs.SetInt("Score" + LevelManager.Instance.currentLevel, NetworkDataManager.LevelScoreCurrentRecord);
            PlayerPrefs.Save();
        });
    }

    #endregion

    #region LEVEL

    public void SetPlayerLevel(int level)
    {
        if (!NetworkManager.Instance.IsLoggedIn)
            return;

        if (level <= LatestReachedLevel)
            return;

        _dataManager.SetPlayerLevel(level);
    }

    public void GetPlayerLevel()
    {
        if (!NetworkManager.Instance.IsLoggedIn)
            return;

        _dataManager.GetPlayerLevel((value) => //2.1.5 Fixed: progress not saved after login
        {
            NetworkDataManager.LatestReachedLevel = value;
            if (NetworkDataManager.LatestReachedLevel <= 0)
                NetworkManager.dataManager.SetPlayerLevel(1);
            GetStars();
        });
    }

    #endregion

    #region STARS

    public void SetStars()
    {
        int level = LevelManager.Instance.currentLevel;
        int stars = PlayerPrefs.GetInt(string.Format("Level.{0:000}.StarsCount", level));
        _dataManager.SetStars(stars, level);
    }

    public void GetStars()
    {
        if (!NetworkManager.Instance.IsLoggedIn)
            return;

        Debug.Log(LevelsMap._instance.GetLastestReachedLevel() + " " + LatestReachedLevel);
        if (LevelsMap._instance.GetLastestReachedLevel() > LatestReachedLevel)
        {
            Debug.Log("reached higher level than synced");
            SyncAllData();
            return;
        }

        _dataManager.GetStars((dic) =>
        {
            foreach (var item in dic)
            {
                PlayerPrefs.SetInt(string.Format("Level.{0:000}.StarsCount", int.Parse(item.Key.Replace("StarsLevel_", ""))), item.Value);
            }
            PlayerPrefs.Save();
            LevelsMap._instance.Reset();

        });
    }

    #endregion

    #region BOOSTS

    public void SetBoosterData()
    {
        Dictionary<string, string> dic = new Dictionary<string, string>() { { "Boost_" + (int) BoostType.ExtraMoves, "" + PlayerPrefs.GetInt ("" + BoostType.ExtraMoves) }, { "Boost_" + (int) BoostType.Packages, "" + PlayerPrefs.GetInt ("" + BoostType.Packages) }, { "Boost_" + (int) BoostType.Stripes, "" + PlayerPrefs.GetInt ("" + BoostType.Stripes) }, { "Boost_" + (int) BoostType.ExtraTime, "" + PlayerPrefs.GetInt ("" + BoostType.ExtraTime) }, { "Boost_" + (int) BoostType.Bomb, "" + PlayerPrefs.GetInt ("" + BoostType.Bomb) }, { "Boost_" + (int) BoostType.Colorful_bomb, "" + PlayerPrefs.GetInt ("" + BoostType.Colorful_bomb) }, { "Boost_" + (int) BoostType.Hand, "" + PlayerPrefs.GetInt ("" + BoostType.Hand) }, { "Boost_" + (int) BoostType.Random_color, "" + PlayerPrefs.GetInt ("" + BoostType.Random_color) }
        };

        _dataManager.SetBoosterData(dic);
    }

    public void GetBoosterData()
    {
        if (!NetworkManager.Instance.IsLoggedIn)
            return;

        _dataManager.GetBoosterData((dic) =>
        {
            foreach (var item in dic)
            {
                PlayerPrefs.SetInt("" + (BoostType)int.Parse(item.Key.Replace("Boost_", "")), item.Value);
            }
            PlayerPrefs.Save();
        });
    }

    #endregion

    public void SetTotalStars()
    {
        LevelsMap._instance.GetMapLevels().Where(l => !l.IsLocked).ToList().ForEach(i => _dataManager.SetStars(i.StarsCount, i.Number)); //2.1.5
    }

    public void SyncAllData()
    {
        SetTotalStars();
        SetPlayerLevel(LevelsMap._instance.GetLastestReachedLevel());
        SetBoosterData();//2.1.5 sync boosters
        SetPlayerScoreTotal();//2.1.6 sync levels
        NetworkManager.currencyManager.SetBalance(PlayerPrefs.GetInt("Gems"));//2.1.5 sync currency

    }

}