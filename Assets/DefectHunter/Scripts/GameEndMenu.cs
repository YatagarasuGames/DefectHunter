using DG.Tweening;
using Firebase.Auth;
using Firebase.Database;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

public class GameEndMenu : MonoBehaviour
{
    [Inject] private PointsSystem _pointsSystem;
    private DatabaseReference _db;
    private FirebaseUser _currentUser;


    [SerializeField] private TMP_Text _earnedPoints;
    private void Start()
    {
        transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        transform.DOScale(new Vector3(1, 1, 1), 0.2f);

        _db = FirebaseDatabase.DefaultInstance.RootReference;
        _currentUser = FirebaseAuth.DefaultInstance.CurrentUser;
        StartCoroutine(UpdateDB());

        _earnedPoints.text = $"Congratulations! You earned {_pointsSystem.GetDecryptedPoints()} for this run!";

    }

    public IEnumerator UpdateDB()
    {
        var task = _db.Child("users").Child(_currentUser.UserId).GetValueAsync();
        yield return new WaitUntil(predicate: () => task.IsCompleted);

        DataSnapshot snapshot = task.Result;
        string jsonResult = snapshot.GetRawJsonValue();

        if (jsonResult != null)
        {
            UserData userData = JsonUtility.FromJson<UserData>(jsonResult);
            int newPoints = userData.Points + _pointsSystem.GetDecryptedPoints();

            UserData newUserData = new UserData(userData.Username, newPoints);
            string userDataJson = JsonUtility.ToJson(newUserData);
            _db.Child("users").Child(_currentUser.UserId).SetRawJsonValueAsync(userDataJson);
            print(newPoints);
        }

    }

    public void Exit()
    {
        DOTween.Sequence().Kill();
        SceneManager.LoadScene("Menu");
    }


}
