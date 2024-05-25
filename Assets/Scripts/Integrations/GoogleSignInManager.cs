using Google;
using UnityEngine;
using UnityEngine.UI;

namespace Dev.Scripts.Integrations
{
    public class SignInWithGoogle : MonoBehaviour
    {
        public Text statusText;
        private FirebaseManager firebaseManager;

        void Start()
        {
            firebaseManager = new FirebaseManager();
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
                    firebaseManager.LoginWithGoogle(googleUser.IdToken, statusText);
                }
            });
        }
    }

}