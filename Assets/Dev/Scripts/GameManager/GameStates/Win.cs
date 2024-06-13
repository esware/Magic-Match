using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

namespace GameStates
{
    public class Win:GameState
    {
        private GameManager _gameManager;
        public override void EnterState()
        {
            _gameManager = GameManager.Instance;
            Debug.Log("Entering Win State");

            if (!InitScript.Instance.losingLifeEveryGame)
                InitScript.Instance.AddLife(1);

            GameEvents.OnMenuComplete?.Invoke();
            var playerPrefs = new PlayerPrefsMapProgressManager();
            int currentStars = playerPrefs.LoadLevelStarsCount(_gameManager.currentLevel);

            if (currentStars < _gameManager.stars)
            {
                playerPrefs.SaveLevelStarsCount(_gameManager.currentLevel, _gameManager.stars);
                PlayerPrefs.Save();
                Debug.Log("Stars count updated and saved.");
            }

            if (GameManager.Score > playerPrefs.LoadLevelScoreCount(_gameManager.currentLevel))
            {
                playerPrefs.SaveLevelScoreCount(_gameManager.currentLevel,GameManager.Score);
                PlayerPrefs.Save();
                Debug.Log("Score updated and saved.");
            }

#if PLAYFAB
    NetworkManager.dataManager.SetPlayerScore(currentLevel, Score);
    NetworkManager.dataManager.SetPlayerLevel(currentLevel + 1);
    NetworkManager.dataManager.SetStars();
#endif

            GameObject.Find("CanvasGlobal").transform.Find("MenuComplete").gameObject.SetActive(true);
            SoundBase.Instance.GetComponent<AudioSource>().PlayOneShot(SoundBase.Instance.complete[1]);
            GameEvents.OnWin?.Invoke();
        }


        public override void UpdateState()
        {
            
        }

        public override void ExitState()
        {
            
        }
    }
}