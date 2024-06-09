using System.Collections;
using UnityEngine;

namespace GameStates
{
    public class Map:GameState
    {
        public override void EnterState()
        {
            Debug.Log("Map State");
            if (PlayerPrefs.GetInt(PlayerPrefsKeys.OpenLevelTest) <= 0)
            {
                MusicBase.Instance.GetComponent<AudioSource>().Stop();
                MusicBase.Instance.GetComponent<AudioSource>().loop = true;
                MusicBase.Instance.GetComponent<AudioSource>().clip = MusicBase.Instance.music[0];
                MusicBase.Instance.GetComponent<AudioSource>().Play();
                GameManager.Instance.EnableMap(true);
                GameEvents.OnMapState?.Invoke();
            }
            else
            {
                PlayerPrefs.SetInt(PlayerPrefsKeys.OpenLevelTest, 0);
                PlayerPrefs.Save();
            }

        }

        public override void UpdateState()
        {
            
        }

        public override void ExitState()
        {
            
        }
    }
}