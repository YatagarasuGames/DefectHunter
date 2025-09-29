using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private Transform _canvas;
    [SerializeField] private GameObject _exitConfirm;
    [SerializeField] private GameObject _profileViewWindow;
    public void StartGame()
    {
        SceneManager.LoadScene("Game");
    }

    public void OpenProfileViewMenu()
    {
        Instantiate(_profileViewWindow, _canvas);
    }

    public void Exit()
    {
        Instantiate(_exitConfirm, _canvas);
    }
}
