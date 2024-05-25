using Firebase.Extensions;
using Google;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Dev.Scripts.Integrations
{
    public class GoogleSignInManager : MonoBehaviour
    {
        public TextMeshProUGUI statusText;
        void Start()
        {
            ConfigureGoogleSignIn();
        }

        private void ConfigureGoogleSignIn()
        {
            GoogleSignIn.Configuration = new GoogleSignInConfiguration
            {
                WebClientId = "YOUR_WEB_CLIENT_ID",
                RequestIdToken = true
            };
        }

        public void SignInWithGoogle()
        {
            GoogleSignIn.DefaultInstance.SignIn().ContinueWithOnMainThread(task =>
            {
                if (task.IsCanceled || task.IsFaulted)
                {
                    statusText.text = "Google Sign-In failed.";
                }
                else
                {
                    var googleUser = task.Result;
                    FirebaseManager.Instance.LoginWithGoogle(googleUser.IdToken, statusText);
                }
            });
        }
    }


}