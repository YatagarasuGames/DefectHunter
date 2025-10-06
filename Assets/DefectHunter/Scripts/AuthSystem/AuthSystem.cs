using TMPro;
using UnityEngine;
using Firebase;
using Firebase.Auth;
using Firebase.Extensions;
using System.Collections;
using UnityEngine.SceneManagement;
using System;
using Firebase.Database;
using UnityEngine.UI;

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
    [SerializeField] private TMP_Text _loginErrorLog;
    [SerializeField] private Toggle _rememberMeToggle;

    [Header("Register")]
    [SerializeField] private TMP_InputField _usernameInput;
    [SerializeField] private TMP_InputField _registerEmailInput;
    [SerializeField] private TMP_InputField _registerPasswordInput;
    [SerializeField] private TMP_InputField _registerConfirmPasswordInput;
    [SerializeField] private TMP_Text _registerErrorLog;

    private const string REMEMBER_ME_KEY = "RememberMe";
    private const string LAST_LOGIN_DATE_KEY = "LastLoginDate";
    private const int DAYS_TO_EXPIRE = 3;

    private DatabaseReference _db;

    private void Awake()
    {
        _db = FirebaseDatabase.DefaultInstance.RootReference;
        CheckAutoLogin();
    }

    private void Start()
    {
        _rememberMeToggle.isOn = PlayerPrefs.GetInt(REMEMBER_ME_KEY, 0) == 1;
    }

    private void CheckAutoLogin()
    {
        if (ShouldAutoLogin())
        {
            Debug.Log("Attempting auto-login...");
            AutoLogin();
        }
        else
        {
            Debug.Log("Auto-login not available, showing login screen");
        }
    }

    private bool ShouldAutoLogin()
    {
        if (PlayerPrefs.GetInt(REMEMBER_ME_KEY, 0) == 0)
            return false;

        string lastLoginString = PlayerPrefs.GetString(LAST_LOGIN_DATE_KEY, "");
        if (string.IsNullOrEmpty(lastLoginString))
            return false;

        try
        {
            DateTime lastLoginDate = DateTime.Parse(lastLoginString);
            TimeSpan timeSinceLastLogin = DateTime.Now - lastLoginDate;

            if (timeSinceLastLogin.TotalDays <= DAYS_TO_EXPIRE)
            {
                Debug.Log($"Auto-login available");
                return true;
            }
            else
            {
                Debug.Log($"Auto-login expired");
                return false;
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Error parsing login date: {e}");
            return false;
        }
    }

    private void AutoLogin()
    {
        FirebaseAuth auth = FirebaseAuth.DefaultInstance;

        FirebaseUser currentUser = auth.CurrentUser;

        if (currentUser != null)
        {
            Debug.Log("User already authenticated, proceeding to menu");
            OnAutoLoginSuccess(currentUser);
        }
        else
        {
            StartCoroutine(WaitForAuth());
        }
    }

    private IEnumerator WaitForAuth()
    {
        Debug.Log("Waiting for Firebase Auth initialization...");

        var auth = FirebaseAuth.DefaultInstance;
        yield return new WaitForSeconds(1f);

        var user = auth.CurrentUser;

        if (user != null)
        {
            Debug.Log($"Auto-login successful for user: {user.Email}");
            OnAutoLoginSuccess(user);
        }
        else
        {
            Debug.Log("No saved session found for auto-login");
            ClearRememberMe();
        }
    }

    private void OnAutoLoginSuccess(FirebaseUser user)
    {
        PlayerPrefs.SetString(LAST_LOGIN_DATE_KEY, DateTime.Now.ToString());
        PlayerPrefs.Save();

        LoadMainMenu();
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
                foreach (Exception exception in task.Exception.Flatten().InnerExceptions)
                {
                    if (exception is FirebaseException firebaseEx)
                    {
                        HandleFirebaseAuthError(firebaseEx, _registerErrorLog);
                        _registerErrorLog.gameObject.SetActive(true);
                    }
                    else
                    {
                        Debug.LogError($"Unexpected error: {exception.Message}");
                    }
                }
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
                SaveRememberMeState(true);
                SceneManager.LoadScene("Menu");
            }
            else
            {
                Debug.Log("Email verification required");
                SendEmailVerification();
                SaveRememberMeState(true);
            }

        });
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
                return;
            }
            if (task.IsFaulted)
            {
                foreach (Exception exception in task.Exception.Flatten().InnerExceptions)
                {
                    if (exception is FirebaseException firebaseEx)
                    {
                        HandleFirebaseAuthError(firebaseEx, _loginErrorLog);
                        _loginErrorLog.gameObject.SetActive(true);
                    }
                    else
                    {
                        Debug.LogError($"Unexpected error: {exception.Message}");
                    }
                }
                return;
            }

            AuthResult result = task.Result;

            print(task.Result.User.IsEmailVerified);
            print(result.User.DisplayName);
            print(result.User.UserId);

            if (result.User.IsEmailVerified)
            {
                Debug.Log("Login success");
                SaveRememberMeState(_rememberMeToggle.isOn);
                SceneManager.LoadScene("Menu");
            }
            else
            {
                Debug.LogWarning("Please verify email");
                SaveRememberMeState(_rememberMeToggle.isOn);
            }
        });
    }

    private void SaveRememberMeState(bool rememberMe)
    {
        if (rememberMe)
        {
            PlayerPrefs.SetInt(REMEMBER_ME_KEY, 1);
            PlayerPrefs.SetString(LAST_LOGIN_DATE_KEY, DateTime.Now.ToString());
        }
        else
        {
            PlayerPrefs.SetInt(REMEMBER_ME_KEY, 0);
            PlayerPrefs.DeleteKey(LAST_LOGIN_DATE_KEY);
        }
        PlayerPrefs.Save();
        Debug.Log($"Remember me set to: {rememberMe}");
    }

    private void ClearRememberMe()
    {
        PlayerPrefs.SetInt(REMEMBER_ME_KEY, 0);
        PlayerPrefs.DeleteKey(LAST_LOGIN_DATE_KEY);
        PlayerPrefs.Save();
        Debug.Log("Remember me cleared");
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

    private void LoadMainMenu()
    {
        SceneManager.LoadScene("Menu");
    }



    private void HandleFirebaseAuthError(FirebaseException firebaseEx, TMP_Text errorLog)
    {
        
        AuthError errorCode = (AuthError)firebaseEx.ErrorCode;

        switch (errorCode)
        {
            case AuthError.InvalidEmail:
                errorLog.text = "Invalid email address";
                break;
            case AuthError.WrongPassword:
                errorLog.text = "Wrong password";
                break;
            case AuthError.MissingPassword:
                errorLog.text = "Password is missing";
                break;
            case AuthError.WeakPassword:
                errorLog.text = "Password is too weak";
                break;
            case AuthError.UserNotFound:
                errorLog.text = "User not found";
                break;
            case AuthError.EmailAlreadyInUse:
                errorLog.text = "Email already in use";
                break;
            case AuthError.OperationNotAllowed:
                errorLog.text = "Operation not allowed";
                break;
            case AuthError.TooManyRequests:
                errorLog.text = "Too many requests. Try again later";
                break;
            case AuthError.AccountExistsWithDifferentCredentials:
                errorLog.text = "Account exists with different credentials";
                break;
            case AuthError.RequiresRecentLogin:
                errorLog.text = "Requires recent login";
                break;
            case AuthError.UserDisabled:
                errorLog.text = "User account is disabled";
                break;
            case AuthError.NetworkRequestFailed:
                errorLog.text = "Network request failed";
                break;
            default:
                errorLog.text = $"Unknown error: {errorCode}";
                break;
        }
    }

}