
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
#if UNITY_ANALYTICS
using UnityEngine.Analytics;
#endif

namespace Dev.Scripts.Integrations
{
    public class EventsListener : MonoBehaviour {

        void OnEnable() {
            GameEvents.OnMapState += OnMapState;
            GameEvents.OnEnterGame += OnEnterGame;
            GameEvents.OnLevelLoaded += OnLevelLoaded;
            GameEvents.OnMenuPlay += OnMenuPlay;
            GameEvents.OnMenuComplete += OnMenuComplete;
            GameEvents.OnStartPlay += OnStartPlay;
            GameEvents.OnWin += OnWin;
            GameEvents.OnLose += OnLose;

        }

        void OnDisable() {
            GameEvents.OnMapState -= OnMapState;
            GameEvents.OnEnterGame -= OnEnterGame;
            GameEvents.OnLevelLoaded -= OnLevelLoaded;
            GameEvents.OnMenuPlay -= OnMenuPlay;
            GameEvents.OnMenuComplete -= OnMenuComplete;
            GameEvents.OnStartPlay -= OnStartPlay;
            GameEvents.OnWin -= OnWin;
            GameEvents.OnLose -= OnLose;

        }

        #region GAME_EVENTS
        void OnMapState() {
        }
        void OnEnterGame() {
            AnalyticsEvent("OnEnterGame", LevelManager.Instance.currentLevel);
        }
        void OnLevelLoaded() {
        }
        void OnMenuPlay() {
        }
        void OnMenuComplete() {
        }
        void OnStartPlay() {
        }
        void OnWin() {
            AnalyticsEvent("OnWin", LevelManager.Instance.currentLevel);
        }
        void OnLose() {
            AnalyticsEvent("OnLose", LevelManager.Instance.currentLevel);
        }

        #endregion

        void AnalyticsEvent(string _event, int level) {
#if UNITY_ANALYTICS
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add(_event, level);
            Analytics.CustomEvent(_event, dic);

#endif
        }


    }
}