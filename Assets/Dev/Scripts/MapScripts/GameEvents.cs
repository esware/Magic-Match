using System;
using JetBrains.Annotations;
using UnityEngine;

namespace MapScripts.Scripts
{
    public struct GameEvents
    {
        public static Action OnMapState;
        public static Action OnEnterGame;
        public static Action OnLevelLoaded;
        public static Action OnMenuPlay;
        public static Action OnMenuComplete;
        public static Action OnStartPlay;
        public static Action OnWin;
        public static Action OnLose;

        public static void ResetEvents()
        {
            OnMapState = null;
            OnEnterGame = null;
            OnLevelLoaded = null;
            OnMenuPlay = null;
            OnMenuComplete = null;
            OnStartPlay = null;
            OnWin = null;
            OnLose = null;
        }
    }
}