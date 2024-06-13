using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Dev.Scripts.Integrations;

namespace Dev.Scripts.GUI.Avatar
{
    public class AvatarManager : MonoBehaviour {
        public List<GameObject> avatars = new List<GameObject> ();

        void OnEnable () 
        {
#if PLAYFAB 
		NetworkManager.OnFriendsOnMapLoaded += CheckFriendsList;

#endif
        }

        void OnDisable () 
        {
#if PLAYFAB 
		NetworkManager.OnFriendsOnMapLoaded -= CheckFriendsList;
#endif
        }
    }
}