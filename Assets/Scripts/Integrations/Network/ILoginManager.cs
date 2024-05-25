using TMPro;
using UnityEngine.UI;

namespace Dev.Scripts.Integrations.Network
{
    public interface ILoginManager
    {
        void Register(string email,string username, string password, TextMeshProUGUI statusText);
        void Login(string email, string password, TextMeshProUGUI statusText);
        void LoginWithGoogle(string idToken, TextMeshProUGUI statusText);
        void UpdateName(string userName);
        bool IsYou(string userId);
        void Logout();
    }

}