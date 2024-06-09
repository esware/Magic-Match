using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;


namespace Dev.Scripts.GUI.Avatar
{
    public class PlayerAvatar : MonoBehaviour, IAvatarLoader {
        public Image image;

        void Start () {
            image.enabled = false;
        }

#if PLAYFAB
	void OnEnable () {
		NetworkManager.OnPlayerPictureLoaded += ShowPicture;
	}

	void OnDisable () {
		NetworkManager.OnPlayerPictureLoaded -= ShowPicture;
	}


#endif
        public void ShowPicture () {
            image.sprite = InitScript.ProfilePic;
            image.enabled = true;
        }

    }
}