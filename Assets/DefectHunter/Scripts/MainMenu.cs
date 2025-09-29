using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private Transform _canvas;
    [SerializeField] private GameObject _exitConfirm;
    public void StartGame()
    {
        SceneManager.LoadScene("Game");
    }

    public void Exit()
    {
        Instantiate(_exitConfirm, _canvas);
    }
}
