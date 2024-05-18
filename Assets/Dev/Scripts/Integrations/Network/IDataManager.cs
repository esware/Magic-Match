﻿using System.Collections.Generic;
using System;

namespace Dev.Scripts.Integrations
{
    public interface IDataManager {
        void SetPlayerScore (int level, int score);

        void SetPlayerLevel (int level) ;

        void GetPlayerLevel (Action<int> Callback);

        void GetPlayerScore (Action<int> Callback);

        void SetStars (int Stars, int Level);

        void GetStars (Action<Dictionary<string,int>> Callback);

        void SetTotalStars ();

        void SetBoosterData (Dictionary<string, string> dic);

        void GetBoosterData (Action<Dictionary<string,int>> Callback);

        void Logout ();
    }
}