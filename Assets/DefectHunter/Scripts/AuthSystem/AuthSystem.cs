using TMPro;
using UnityEngine;
using Firebase;
using Firebase.Auth;
using Firebase.Extensions;
using System.Collections;
using UnityEngine.SceneManagement;
using System;
using Firebase.Database;

[Serializable]
public class UserData
{
    public string Username;
    public int Points;

    public UserData(string username, int points)
    {
        Username = username; Points = points;
    }
}

public class AuthSystem : MonoBehaviour
{
    [Header("Login")]
    [SerializeField] private TMP_InputField _loginEmailInput;
    [SerializeField] private TMP_InputField _loginPasswordInput;

    [Header("Register")]
    [SerializeField] private TMP_InputField _usernameInput;
    [SerializeField] private TMP_InputField _registerEmailInput;
    [SerializeField] private TMP_InputField _registerPasswordInput;
    [SerializeField] private TMP_InputField _registerConfirmPasswordInput;

    private DatabaseReference _db;
    private void Awake()
    {
        _db = FirebaseDatabase.DefaultInstance.RootReference;
    }

    public void Register()
    {
        FirebaseAuth auth = FirebaseAuth.DefaultInstance;
        string email = _registerEmailInput.text;
        string password = _registerPasswordInput.text;
        string confirmedPassword = _registerConfirmPasswordInput.text;

        auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("Register Canceled");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("Register Failed");
                print(task.Exception);
                return;
            }
            
            AuthResult result = task.Result;
            Debug.LogFormat("Created user: {0}, {1}",
                result.User.DisplayName, result.User.UserId);

            UserData userData = new UserData(_usernameInput.text, 0);
            string userDataJson = JsonUtility.ToJson(userData);
            _db.Child("users").Child(result.User.UserId).SetRawJsonValueAsync(userDataJson);    
            
            if (result.User.IsEmailVerified)
            {
                Debug.Log("Register success");
                SceneManager.LoadScene("Menu");
            }
            else
            {
                Debug.Log("Email verification required");
                SendEmailVerification();
            }

        });
    }
    public void SendEmailVerification()
    {
        StartCoroutine(SendEmailVerificationAsync());
    }

    private IEnumerator SendEmailVerificationAsync()
    {
        FirebaseUser user = FirebaseAuth.DefaultInstance.CurrentUser;

        if (user != null)
        {
            var sendEmailTask = user.SendEmailVerificationAsync();
            yield return new WaitUntil(() => sendEmailTask.IsCompleted);

            if (sendEmailTask.Exception != null)
            {
                print("Email send error");
            }
            else
            {
                print("Email successfully sent");
            }
        }
    }



    public void Login()
    {
        FirebaseAuth auth = FirebaseAuth.DefaultInstance;
        string email = _loginEmailInput.text;
        string password = _loginPasswordInput.text;

        Credential credential = EmailAuthProvider.GetCredential(email, password);

        auth.SignInAndRetrieveDataWithCredentialAsync(credential).ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("Login cancelled");
            }
            if (task.IsFaulted)
            {
                Debug.LogError("Login failed");
                print(task.Exception.ToString());
            }
            
            AuthResult result = task.Result;

            print(task.Result.User.IsEmailVerified);
            print(result.User.DisplayName);
            print(result.User.UserId);

            if (result.User.IsEmailVerified)
            {
                Debug.Log("Login success");
                SceneManager.LoadScene("Menu");
            }
            else
            {
                Debug.LogWarning("Please verify email");
            }


        });
        print("end");
    }
}
