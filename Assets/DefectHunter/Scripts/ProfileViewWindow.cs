using DG.Tweening;
using Firebase.Auth;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ProfileViewWindow : MonoBehaviour
{
    [SerializeField] private TMP_Text _username;
    [SerializeField] private TMP_Text _points;
    private void OnEnable()
    {
        transform.localScale = Vector3.zero;
        transform.DOScale(new Vector3(1, 1, 1), 0.1f);
        MenuUserDataLoader loader = GameObject.FindFirstObjectByType<MenuUserDataLoader>();
        StartCoroutine(loader.GetUserDataCoroutine((userData) =>
        {
            _username.text = userData.Username;
            _points.text = userData.Points.ToString();
        }));
    }

    public void Logout()
    {
        FirebaseAuth auth = FirebaseAuth.DefaultInstance;
        auth.SignOut();

        ClearRememberMe();
        SceneManager.LoadScene("Auth");
    }

    private void ClearRememberMe()
    {
        PlayerPrefs.SetInt("RememberMe", 0);
        PlayerPrefs.DeleteKey("LastLoginDate");
        PlayerPrefs.Save();
        Debug.Log("Remember me cleared");
    }

    public void Close()
    {
        DOTween.Sequence().Kill();
        Destroy(gameObject);
    }
}
