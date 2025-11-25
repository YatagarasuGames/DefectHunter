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
        _username.text = PlayerPrefs.GetString("Nickname");
        _points.text = "0";
    }

    public void Logout()
    {
        ApiService.Instance.Logout();
    }

    public void Close()
    {
        DOTween.Sequence().Kill();
        Destroy(gameObject);
    }
}
