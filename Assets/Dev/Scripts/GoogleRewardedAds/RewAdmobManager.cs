using System;
using UnityEngine;
using GoogleMobileAds.Api;
namespace Dev.Scripts.GoogleRewardedAds
{
    public class RewAdmobManager:MonoBehaviour
    {
        public static RewAdmobManager Instance;
        private RewardedAd _rewardVideoAd;
        private Action _resultCallBack;

        private void Awake()
        {
            if (Instance==null)
                Instance = this;
            else if (Instance!=this)
                Destroy(gameObject);
            DontDestroyOnLoad(this);
        }

        private void Start()
        {
           
        }
    }
}