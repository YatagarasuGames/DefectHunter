using Firebase.Auth;
using Firebase.Database;
using System.Collections;
using TMPro;
using UnityEngine;

public class MenuUserDataLoader : MonoBehaviour
{
    [SerializeField] private TMP_Text _username;
    [SerializeField] private TMP_Text _points;

    private DatabaseReference db;
    private FirebaseUser currentUser;
    private void OnEnable()
    {
        db = FirebaseDatabase.DefaultInstance.RootReference;
        currentUser = FirebaseAuth.DefaultInstance.CurrentUser;
        StartCoroutine(GetUserData());
    }

    private IEnumerator GetUserData()
    {
        var task = db.Child("users").Child(currentUser.UserId).GetValueAsync();
        yield return new WaitUntil(predicate: () => task.IsCompleted);

        DataSnapshot snapshot = task.Result;
        string jsonResult = snapshot.GetRawJsonValue();

        if(jsonResult != null)
        {
            UserData userData = JsonUtility.FromJson<UserData>(jsonResult);
            _username.text = userData.Username;
            _points.text = userData.Points.ToString();
        }
    }
}
