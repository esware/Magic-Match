using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

namespace GameStates
{
    public class Win:GameState
    {
        private GameManager _gameManager;
        public override void EnterState()
        {
            _gameManager= GameManager.Instance;
            
            if (!InitScript.Instance.losingLifeEveryGame)
                InitScript.Instance.AddLife(1);
            
            GameEvents.OnMenuComplete?.Invoke();
            
            if (PlayerPrefs.GetInt(string.Format("Level.{0:000}.StarsCount", _gameManager.currentLevel), 0) < _gameManager.stars)
                PlayerPrefs.SetInt(string.Format("Level.{0:000}.StarsCount", _gameManager.currentLevel), _gameManager.stars);
            if (GameManager.Score > PlayerPrefs.GetInt("Score" + _gameManager.currentLevel))
            {
                PlayerPrefs.SetInt("Score" + _gameManager.currentLevel, GameManager.Score);
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
            throw new System.NotImplementedException();
        }

        public override void ExitState()
        {
            throw new System.NotImplementedException();
        }
    }
}