using System.Collections.Generic;
using Dev.Scripts.Integrations.Network;
using PlayFab;
using PlayFab.ClientModels;
using TMPro;
using UnityEngine;

public class PlayFabManager : ILoginManager
{
    private static PlayFabManager _instance;

    public static PlayFabManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new PlayFabManager();
            }
            return _instance;
        }
    }

    private PlayFabManager() { }

    private void OnLoginSuccess(LoginResult result)
    {
        Debug.Log("PlayFab login successful: " + result.PlayFabId);
        NetworkManager.Instance.IsLoggedIn = true;
        NetworkManager.UserID = result.PlayFabId;
    }


    private void OnLoginFailure(PlayFabError error)
    {
        Debug.LogError("PlayFab login failed: " + error.GenerateErrorReport());
    }

    public void Register(string email, string username, string password, TextMeshProUGUI statusText)
    {
        var request = new RegisterPlayFabUserRequest
        {
            Email = email,
            Username = username,
            Password = password,
            RequireBothUsernameAndEmail = true
        };
        PlayFabClientAPI.RegisterPlayFabUser(request, result =>
        {
            Debug.Log("PlayFab registration successful: " + result.PlayFabId);
            statusText.text = "Registration successful!";
        }, error =>
        {
            Debug.LogError("PlayFab registration failed: " + error.GenerateErrorReport());
            statusText.text = "Registration failed: " + error.GenerateErrorReport();
        });
    }

    public void Login(string email, string password, TextMeshProUGUI statusText)
    {
        var request = new LoginWithEmailAddressRequest
        {
            Email = email,
            Password = password
        };
        PlayFabClientAPI.LoginWithEmailAddress(request, result =>
        {
            OnLoginSuccess(result);
            statusText.text = "Login successful!";
        }, error =>
        {
            OnLoginFailure(error);
            statusText.text = "Login failed: " + error.GenerateErrorReport();
        });
    }

    public void LoginWithGoogle(string idToken, TextMeshProUGUI statusText)
    {
        throw new System.NotImplementedException();
    }

    public void UpdateName(string userName)
    {
        var request = new UpdateUserTitleDisplayNameRequest
        {
            DisplayName = userName
        };
        PlayFabClientAPI.UpdateUserTitleDisplayName(request, result =>
        {
            Debug.Log("User name updated to: " + result.DisplayName);
        }, error =>
        {
            Debug.LogError("Failed to update user name: " + error.GenerateErrorReport());
        });
    }

    public bool IsYou(string userId)
    {
        return NetworkManager.UserID == userId;
    }

    public void Logout()
    {
        PlayFabClientAPI.ForgetAllCredentials();
        NetworkManager.Instance.IsLoggedIn = false;
    }

    public void UpdateUserData(string key, string value)
    {
        var request = new UpdateUserDataRequest
        {
            Data = new Dictionary<string, string>
            {
                { key, value }
            }
        };
        PlayFabClientAPI.UpdateUserData(request, result =>
        {
            Debug.Log("Successfully updated user data");
        }, error =>
        {
            Debug.LogError("Failed to update user data: " + error.GenerateErrorReport());
        });
    }
}
