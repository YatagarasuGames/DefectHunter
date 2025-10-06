using Firebase.Database;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LeaderBoardLoader : MonoBehaviour
{
    private DatabaseReference _db;

    [SerializeField] private GameObject _dbUserTemplate;
    [SerializeField] private Transform _content;

    private void Awake()
    {
        _db = FirebaseDatabase.DefaultInstance.RootReference;
    }

    private void OnEnable()
    {
        StartCoroutine(InitLeaderboard());
    }



    private IEnumerator InitLeaderboard()
    {
        List<UserData> userData = new List<UserData>();

        var task = _db.Child("users").GetValueAsync();
        yield return new WaitUntil(predicate: () => task.IsCompleted);
        DataSnapshot snapshot = task.Result;
        
        if (snapshot.Exists)
        {
            foreach (DataSnapshot userSnapshot in snapshot.Children)
            {
                string userJson = userSnapshot.GetRawJsonValue();
                UserData user = JsonUtility.FromJson<UserData>(userJson);
                userData.Add(user);
            }
        }
        
        userData = userData.OrderByDescending(user => user.Points).ToList();

        for (int i = 0; i < userData.Count; i++)
        {
            UserData currentUser = userData[i];
            GameObject currentUserRecord = Instantiate(_dbUserTemplate, _content);
            currentUserRecord.GetComponent<UserLeaderBoardTemplate>().Init(i + 1, currentUser.Username, currentUser.Points);
        }
    }

    private void CleanChildren()
    {
        for (int i = 0; i < _content.childCount; i++)
        {
            Destroy(_content.GetChild(0).gameObject);
        }
    }
    private void OnDisable()
    {
        CleanChildren();
    }
}
