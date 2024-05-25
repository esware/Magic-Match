using Dev.Scripts.Integrations;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Dev.Scripts.GUI
{
    public class LoginUI : MonoBehaviour
    {
        public TMP_InputField emailInput;
        public TMP_InputField passwordInput;
        public TMP_InputField usernameInput;
        public TextMeshProUGUI statusText;
        
        private GoogleSignInManager _googleSignInManager;

        void Start()
        {
            _googleSignInManager = FindObjectOfType<GoogleSignInManager>();
        }

        public void OnRegisterButtonClicked()
        {
            NetworkManager.Instance.RegisterWithFirebase(emailInput.text, usernameInput.text,passwordInput.text, statusText);
        }

        public void OnLoginButtonClicked()
        {
            NetworkManager.Instance.LoginWithFirebase(emailInput.text, passwordInput.text, statusText);
        }

        public void OnGoogleSignInButtonClicked()
        {
            _googleSignInManager.SignInWithGoogle();
        }
        public void OnLogoutButtonClicked()
        {
            NetworkManager.Instance.Logout();
        }
    }



}