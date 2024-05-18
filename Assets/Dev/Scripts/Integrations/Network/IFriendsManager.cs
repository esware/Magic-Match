﻿using System.Collections;
using System.Collections.Generic;
using System;

namespace Dev.Scripts.Integrations.Network
{
    public interface IFriendsManager {
        void GetFriends (Action<Dictionary<string,string>> Callback) ;

        void PlaceFriendsPositionsOnMap (Action<Dictionary<string,int>> Callback);

        void GetLeadboardOnLevel (int LevelNumber, Action<List<LeadboardPlayerData>> Callback);

        void Logout ();
    }
}