using System;
using Dev.Scripts.GUI;
using Dev.Scripts.Targets;
using UnityEngine;
using UnityEngine.UI;

namespace GameStates
{
    public class PrepareGame:GameState
    {
        public override void EnterState()
        {
            GameManager.Instance.passLevelCounter++;

            MusicBase.Instance.GetComponent<AudioSource>().Stop();
            MusicBase.Instance.GetComponent<AudioSource>().loop = true;
            MusicBase.Instance.GetComponent<AudioSource>().clip = MusicBase.Instance.music[1];
            MusicBase.Instance.GetComponent<AudioSource>().Play();
            
            GameManager.Instance.PrepareGame();
        }

        public override void UpdateState()
        {
            
        }

        public override void ExitState()
        {
            
        }
        
        
    }
}