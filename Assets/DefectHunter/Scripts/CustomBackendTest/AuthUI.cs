using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AuthUI : MonoBehaviour
{
    [SerializeField] private AuthManager manager;

    [Header("Login References")]
    public TMP_InputField loginEmailInput;
    public TMP_InputField loginPasswordInput;
    public Button loginButton;

    [Header("Register References")]
    public TMP_InputField registerUsernameInput;
    public TMP_InputField registerEmailInput;
    public TMP_InputField registerPasswordInput;
    public Button registerButton;

    [Header("UI Panels")]
    public GameObject loginPanel;
    public GameObject registerPanel;

    [Header("Message Display")]
    public TextMeshProUGUI messageText;

    private void Start()
    {
        // Настройка кнопок
        loginButton.onClick.AddListener(OnLoginClicked);
        registerButton.onClick.AddListener(OnRegisterClicked);

        // Проверяем, не авторизован ли пользователь
        //if (manager.IsUserLoggedIn())
        //{
        //    ShowMainMenu();
        //}
    }

    private void OnLoginClicked()
    {
        string email = loginEmailInput.text;
        string password = loginPasswordInput.text;

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            ShowMessage("Please fill in all fields", false);
            return;
        }

        //manager.Login(email, password, OnAuthComplete);
        ShowMessage("Logging in...", true);
    }

    private void OnRegisterClicked()
    {
        string username = registerUsernameInput.text;
        string email = registerEmailInput.text;
        string password = registerPasswordInput.text;

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            ShowMessage("Please fill in all fields", false);
            return;
        }

        if (password.Length < 6)
        {
            ShowMessage("Password must be at least 6 characters", false);
            return;
        }

        //manager.Register(username, email, password, OnAuthComplete);
        ShowMessage("Registering...", true);
    }

    private void OnAuthComplete(bool success, string message)
    {
        if (success)
        {
            ShowMessage(message, true);
            ShowMainMenu();
        }
        else
        {
            ShowMessage(message, false);
        }
    }

    private void ShowMessage(string message, bool isSuccess)
    {
        messageText.text = message;
        messageText.color = isSuccess ? Color.green : Color.red;
    }

    private void ShowMainMenu()
    {
        // Переход к главному меню или следующей сцене
        Debug.Log("User authenticated successfully!");
        // SceneManager.LoadScene("MainMenu");
    }

    // Методы для переключения между панелями
    public void ShowLoginPanel()
    {
        loginPanel.SetActive(true);
        registerPanel.SetActive(false);
    }

    public void ShowRegisterPanel()
    {
        loginPanel.SetActive(false);
        registerPanel.SetActive(true);
    }
}