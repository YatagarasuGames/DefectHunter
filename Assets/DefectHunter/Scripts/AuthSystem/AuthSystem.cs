using TMPro;
using UnityEngine;
using Firebase;
using Firebase.Auth;
using Firebase.Extensions;
using System.Collections;

public class AuthSystem : MonoBehaviour
{
    [Header("Login")]
    [SerializeField] private TMP_InputField _loginEmailInput;
    [SerializeField] private TMP_InputField _loginPasswordInput;

    [Header("Register")]
    [SerializeField] private TMP_InputField _registerEmailInput;
    [SerializeField] private TMP_InputField _registerPasswordInput;
    [SerializeField] private TMP_InputField _registerConfirmPasswordInput;


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
                Debug.LogError("Canceled");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("Failed");
                return;
            }
            AuthResult result = task.Result;
            Debug.LogFormat("Created user: {0}, {1}",
                result.User.DisplayName, result.User.UserId);

            if (result.User.IsEmailVerified)
            {
                Debug.Log("Register success");
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
}
