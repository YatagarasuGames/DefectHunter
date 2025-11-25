using DG.Tweening;
using Firebase.Auth;
using Firebase.Database;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using Zenject;
using static UnityEditor.Timeline.TimelinePlaybackControls;

[Serializable]
public class AddToScoreRequest 
{
    public string UserId;
    public uint ScoreToAdd;

    public AddToScoreRequest(string id, uint points)
    {
        UserId = id;
        ScoreToAdd = points;
    }
}

public class GameEndMenu : MonoBehaviour
{
    [Inject] private PointsSystem _pointsSystem;
    private DatabaseReference _db;
    private FirebaseUser _currentUser;
    private readonly string BASE_URL = "https://localhost:7000/leaderboard";

    [SerializeField] private TMP_Text _earnedPoints;

    private void Awake()
    {
        Debug.LogWarning(ApiService.Instance.CurrentUserId);
    }
    private void Start()
    {
        // Анимация появления
        transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        transform.DOScale(new Vector3(1, 1, 1), 0.2f);

        // Показываем заработанные очки
        int earnedPoints = _pointsSystem.GetDecryptedPoints();
        _earnedPoints.text = $"Congratulations! You earned {earnedPoints} points for this run!";

        // Отправляем очки на сервер
        StartCoroutine(UpdateScoreInDatabase(earnedPoints));
    }

    private IEnumerator UpdateScoreInDatabase(int points)
    {
        // Проверяем авторизацию
        if (!ApiService.Instance.IsLoggedIn)
        {
            Debug.LogWarning("User not logged in, cannot update score");
            yield break;
        }

        // Получаем UserId из JWT токена
        string userId = ApiService.Instance.CurrentUserId;
        if (string.IsNullOrEmpty(userId))
        {
            Debug.LogError("Cannot get UserId from token");
            yield break;
        }

        // Создаем запрос
        var requestData = new AddToScoreRequest(userId, (uint)points);
        Debug.LogWarning(requestData.UserId);
        Debug.LogWarning(requestData.ScoreToAdd);
        var jsonData = JsonUtility.ToJson(requestData);
        Debug.LogWarning(jsonData);
        Debug.Log($"Sending score update: {jsonData}");

        using (UnityWebRequest request = UnityWebRequest.Put($"{BASE_URL}/add-to-player-score", jsonData))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            // Используем авторизованный запрос с автоматическим обновлением токена
            yield return ApiService.Instance.ExecuteAuthorizedRequest(
                request,
                response =>
                {
                    Debug.Log($"Score updated successfully: {response}");
                },
                error =>
                {
                    Debug.LogError($"Failed to update score: {error}");
                }
            );
        }
    }


    public void Exit()
    {
        DOTween.Sequence().Kill();
        SceneManager.LoadScene("Menu");
    }


}
