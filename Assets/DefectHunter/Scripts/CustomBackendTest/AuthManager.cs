using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

[Serializable]
public class ErrorResponse
{
    public string message;
}

public class AuthManager : MonoBehaviour
{
    private const string BASE_URL = "https://localhost:7000/auth";


    private void Start()
    {
        StartCoroutine(CheckServerAvailability());
    }
    private IEnumerator CheckServerAvailability()
    {
        using (UnityWebRequest request = new UnityWebRequest($"{BASE_URL}/test", "GET"))
        {
            request.downloadHandler = new DownloadHandlerBuffer();
            request.timeout = 5;

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Server is available");
            }
            else
            {
                Debug.LogError($"Server unavailable: {request.error}");
                Debug.LogError($"URL: {BASE_URL}");
            }
        }
    }

    public void Register(string username, string email, string password, Action<bool, string> callback)
    {
        StartCoroutine(RegisterCoroutine(username, email, password, callback));
    }

    public void Login(string email, string password, Action<bool, string> callback)
    {
        StartCoroutine(LoginCoroutine(email, password, callback));
    }

    private IEnumerator RegisterCoroutine(string username, string email, string password, Action<bool, string> callback)
    {
        string fullUrl = $"{BASE_URL}/register";
        Debug.Log($"Starting REGISTER request to: {fullUrl}");
        Debug.Log($"Request data: {username}, {email}, {password}");

        var requestData = new UserRequest
        {
            Username = username,
            Email = email,
            Password = password
        };

        string jsonData = JsonUtility.ToJson(requestData);
        Debug.Log($"JSON data: {jsonData}");

        using (UnityWebRequest request = new UnityWebRequest(fullUrl, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.timeout = 10;

            request.SetRequestHeader("Accept", "application/json");

            yield return request.SendWebRequest();

            Debug.Log($"Request completed: {request.result}");
            Debug.Log($"Response code: {request.responseCode}");
            Debug.Log($"Response: {request.downloadHandler?.text}");

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log($"REGISTER successful");
                callback?.Invoke(true, "Registration successful!");
            }
            else
            {
                Debug.LogError($"REGISTER failed");
            }
        }
    }


    private string GetErrorMessage(UnityWebRequest request)
    {
        try
        {
            var errorResponse = JsonUtility.FromJson<ErrorResponse>(request.downloadHandler.text);
            return errorResponse.message ?? request.error;
        }
        catch
        {
            return request.error ?? "Unknown error occurred";
        }
    }

    private IEnumerator LoginCoroutine(string email, string password, Action<bool, string> callback)
    {
        var requestData = new UserRequest
        {
            Email = email,
            Password = password
        };

        string jsonData = JsonUtility.ToJson(requestData);

        using (UnityWebRequest request = new UnityWebRequest($"{BASE_URL}/login", "POST"))
        {
            request.SetRequestHeader("Content-Type", "application/json");
            request.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(jsonData));
            request.downloadHandler = new DownloadHandlerBuffer();
            request.disposeDownloadHandlerOnDispose = true;

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                print("Success login");
            }
            else
            {
                var errorMessage = GetErrorMessage(request);
                print(errorMessage);
            }
        }
    }
}
