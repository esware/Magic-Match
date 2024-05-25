using Dev.Scripts.Integrations.Network;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class FirebaseManager : ILoginManager
{
    private static FirebaseManager _instance;
    public static FirebaseManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new FirebaseManager();
            }
            return _instance;
        }
    }
    private readonly FirebaseAuth _auth;
    private DatabaseReference _databaseRef;
    
    public FirebaseManager()
    {
        _auth = FirebaseAuth.DefaultInstance;
    }
    private void OnLoginSuccess(FirebaseUser newUser)
    {
        Debug.Log("Firebase login successful: " + newUser.Email);
        NetworkManager.UserID = newUser.UserId;
        NetworkManager.Instance.IsLoggedIn = true;
    }

    public void Register(string email,string username,string password, TextMeshProUGUI statusText)
    {
        _auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled || task.IsFaulted)
            {
                statusText.text = "Register error: " + task.Exception?.Flatten().Message;
                return;
            }

            AuthResult authResult = task.Result;
            FirebaseUser newUser = authResult.User;
            WriteNewUser(newUser.UserId,username,newUser.Email);
            statusText.text = "User registered successfully: " + newUser.Email;
        });
    }

    public void Login(string email, string password, TextMeshProUGUI statusText)
    {
        _auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled || task.IsFaulted)
            {
                statusText.text = "Login error: " + task.Exception?.Flatten().Message;
                return;
            }

            AuthResult authResult = task.Result;
            FirebaseUser newUser = authResult.User;
            statusText.text = "User logged in successfully: " + newUser.Email;
            OnLoginSuccess(newUser);
        });
    }


    public void LoginWithGoogle(string idToken, TextMeshProUGUI statusText)
    {
        Credential credential = GoogleAuthProvider.GetCredential(idToken, null);
        _auth.SignInWithCredentialAsync(credential).ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled || task.IsFaulted)
            {
                statusText.text = "Google login error: " + task.Exception?.Flatten().Message;
                return;
            }

            FirebaseUser newUser = task.Result;
            statusText.text = "User logged in with Google: " + newUser.Email;
            NetworkManager.UserID = newUser.UserId;
            NetworkManager.Instance.IsLoggedIn = true;
        });
    }

    public void UpdateName(string userName)
    {
        FirebaseUser user = _auth.CurrentUser;
        if (user != null)
        {
            UserProfile profile = new UserProfile { DisplayName = userName };
            user.UpdateUserProfileAsync(profile).ContinueWithOnMainThread(task =>
            {
                if (task.IsCanceled || task.IsFaulted)
                {
                    Debug.LogError("Update user name failed: " + task.Exception?.Flatten().Message);
                }
                else
                {
                    Debug.Log("User name updated to: " + userName);
                }
            });
        }
    }

    public bool IsYou(string userId)
    {
        return _auth.CurrentUser != null && _auth.CurrentUser.UserId == userId;
    }

    public void Logout()
    {
        _auth.SignOut();
        NetworkManager.Instance.IsLoggedIn = false;
    }
    
    private void WriteNewUser(string userId, string username, string email)
    {
        User user = new User(username, email);
        string json = JsonUtility.ToJson(user);

        _databaseRef.Child("users").Child(userId).SetRawJsonValueAsync(json);
    }
}

public class User
{
    private string _username;
    private string _email;

    public User() { }

    public User(string username, string email)
    {
        _username = username;
        _email = email;
    }
}
