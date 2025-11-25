using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static ApiService;

[Serializable]
public class ErrorResponse
{
    public string message;
}

[Serializable]
public class RegisterResponse
{
    public string accessToken;
    public string refreshToken;
    public int expiresIn;
}

public class AuthManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject loginPanel;
    public GameObject registerPanel;
    public GameObject loadingPanel;

    public TMP_InputField loginEmailInput;
    public TMP_InputField loginPasswordInput;
    public TMP_InputField registerUsernameInput;
    public TMP_InputField registerEmailInput;
    public TMP_InputField registerPasswordInput;

    public Button loginButton;
    public Button registerButton;
    public Button switchToRegisterButton;
    public Button switchToLoginButton;

    public TextMeshProUGUI loginStatusText;
    public TextMeshProUGUI registerStatusText;
    public TextMeshProUGUI loadingText;

    private void Start()
    {
        InitializeUI();
        AttemptAutoLogin();
    }

    private void InitializeUI()
    {
        loginButton.onClick.AddListener(OnLoginClicked);
        registerButton.onClick.AddListener(OnRegisterClicked);
        switchToRegisterButton.onClick.AddListener(ShowRegisterPanel);
        switchToLoginButton.onClick.AddListener(ShowLoginPanel);

        loginPanel.SetActive(false);
        registerPanel.SetActive(false);
        loadingPanel.SetActive(false);
    }

    private void AttemptAutoLogin()
    {
        ShowLoadingPanel("Checking authentication...");

        if (ApiService.Instance == null)
        {
            Debug.LogError("ApiService instance is null!");
            HideLoadingPanel();
            ShowLoginPanel();
            return;
        }

        if (!ApiService.Instance.HasTokens)
        {
            Debug.Log("No saved tokens found");
            HideLoadingPanel();
            ShowLoginPanel();
            return;
        }

        StartCoroutine(ApiService.Instance.AutoLogin(
            OnAutoLoginSuccess,
            OnAutoLoginError
        ));
    }

    private void OnAutoLoginSuccess(LoginResponse response)
    {
        HideLoadingPanel();
        ShowGamePanel();
        Debug.Log($"Auto-login successful for user: {response.User.Username}");
    }

    private void OnAutoLoginError(string error)
    {
        HideLoadingPanel();
        ShowLoginPanel();
        loginStatusText.text = $"Auto-login failed: {error}";
        Debug.Log($"Auto-login failed: {error}");
    }

    private void OnLoginClicked()
    {
        string email = loginEmailInput.text;
        string password = loginPasswordInput.text;

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            loginStatusText.text = "Please fill all fields";
            return;
        }

        loginStatusText.text = "Logging in...";
        loginButton.interactable = false;
        ShowLoadingPanel("Logging in...");

        StartCoroutine(LoginCoroutine(email, password, OnLoginSuccess, OnLoginError));
    }

    private IEnumerator LoginCoroutine(string email, string password, Action<LoginResponse> onSuccess, Action<string> onError)
    {
        yield return ApiService.Instance.Login(email, password, onSuccess, onError);
    }

    private void OnLoginSuccess(LoginResponse response)
    {
        HideLoadingPanel();
        loginStatusText.text = "Login successful!";
        ShowGamePanel();
        loginButton.interactable = true;
    }

    private void OnLoginError(string error)
    {
        HideLoadingPanel();
        loginStatusText.text = $"Login failed: {error}";
        loginButton.interactable = true;
    }

    private void OnRegisterClicked()
    {
        string username = registerUsernameInput.text;
        string email = registerEmailInput.text;
        string password = registerPasswordInput.text;

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            registerStatusText.text = "Please fill all fields";
            return;
        }

        if (password.Length < 6)
        {
            registerStatusText.text = "Password must be at least 6 characters";
            return;
        }

        registerStatusText.text = "Registering...";
        registerButton.interactable = false;
        ShowLoadingPanel("Creating account...");

        StartCoroutine(RegisterCoroutine(username, email, password, OnRegisterSuccess, OnRegisterError));
    }

    private IEnumerator RegisterCoroutine(string username, string email, string password, Action<RegisterResponseData> onSuccess, Action<string> onError)
    {
        yield return ApiService.Instance.Register(username, email, password, onSuccess, onError);
    }

    private void OnRegisterSuccess(RegisterResponseData response)
    {
        HideLoadingPanel();
        registerStatusText.text = "Registration successful!";

        if (!string.IsNullOrEmpty(response.accessToken) && !string.IsNullOrEmpty(response.refreshToken))
        {
            ApiService.Instance.SaveTokens(new ApiService.TokenData
            {
                accessToken = response.accessToken,
                refreshToken = response.refreshToken
            });

            var userData = ApiService.Instance.DecodeJwtToken(response.accessToken);
            ShowGamePanel();
        }
        else
        {
            ShowLoginPanel();
            loginEmailInput.text = registerEmailInput.text;
            loginStatusText.text = "Registration successful! Please login.";
        }

        registerButton.interactable = true;
    }

    private void OnRegisterError(string error)
    {
        HideLoadingPanel();
        registerStatusText.text = $"Registration failed: {error}";
        registerButton.interactable = true;
    }

    private void ShowLoadingPanel(string message = "Loading...")
    {
        loadingPanel.SetActive(true);
        loadingText.text = message;
    }

    private void HideLoadingPanel()
    {
        loadingPanel.SetActive(false);
    }

    public void ShowLoginPanel()
    {
        loginPanel.SetActive(true);
        registerPanel.SetActive(false);

        loginStatusText.text = "";
        loginEmailInput.text = "";
        loginPasswordInput.text = "";
        loadingText.text = "";

        loginButton.interactable = true;
        registerButton.interactable = true;
    }

    public void ShowRegisterPanel()
    {
        loginPanel.SetActive(false);
        registerPanel.SetActive(true);

        registerStatusText.text = "";
        registerUsernameInput.text = "";
        registerEmailInput.text = "";
        registerPasswordInput.text = "";
        loadingText.text = "";

        loginButton.interactable = true;
        registerButton.interactable = true;
    }

    private void ShowGamePanel()
    {
        SceneManager.LoadScene("Menu");
    }

    public void ShowMessage(string message)
    {
        loginStatusText.text = message;
    }
}
