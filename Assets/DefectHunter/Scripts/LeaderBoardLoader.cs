using System;
using System.Collections;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

public class UserLeaderboardEntryResponse
{
    public Guid UserId { get; }
    public string Username { get; }
    public uint Score { get; }

    public UserLeaderboardEntryResponse(Guid userId, string username, uint score)
    {
        UserId = userId;
        Username = username;
        Score = score;
    }
}

public class LeaderBoardLoader : MonoBehaviour
{

    [SerializeField] private GameObject _dbUserTemplate;
    [SerializeField] private Transform _content;

    private readonly string BASE_URL = "https://localhost:7000/leaderboard";

    private void OnEnable()
    {
        StartCoroutine(InitLeaderboard());
    }



    private IEnumerator InitLeaderboard()
    {
        using (UnityWebRequest request = new UnityWebRequest($"{BASE_URL}/get-all-users", "GET"))
        {
            request.downloadHandler = new DownloadHandlerBuffer();
            request.disposeDownloadHandlerOnDispose = true;
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                print(request.downloadHandler.text);
                var leaderboard = JsonConvert.DeserializeObject<UserLeaderboardEntryResponse[]>(request.downloadHandler.text);

                for(int i = 0; i < leaderboard.Length; i++)
                {
                    GameObject entryGameobject = Instantiate(_dbUserTemplate, _content);
                    entryGameobject.GetComponent<UserLeaderBoardTemplate>().Init(i+1, leaderboard[i].Username, leaderboard[i].Score);
                }
            }

            else
            {
                print("error");
            }
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
