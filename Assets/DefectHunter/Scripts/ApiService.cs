using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class ApiService : MonoBehaviour
{
    private string baseUrl = "https://localhost:7000";
    private string accessToken;
    private string refreshToken;
    private bool isRefreshingToken = false;

    public static ApiService Instance { get; private set; }

    [System.Serializable]
    public class TokenData
    {
        public string accessToken;
        public string refreshToken;
    }

    [System.Serializable]
    private class RefreshRequestData
    {
        public string token;
    }

    [System.Serializable]
    private class RefreshResponseData
    {
        public string accessToken;
        public string refreshToken;
        public int expiresIn;
        public string id;
    }

    [System.Serializable]
    private class LoginRequestData
    {
        public string username;
        public string email;
        public string password;
    }

    [System.Serializable]
    private class LoginResponseData
    {
        public string accessToken;
        public string refreshToken;
        public int expiresIn;
        public string id;
    }

    [System.Serializable]
    private class RegisterRequestData
    {
        public string username;
        public string email;
        public string password;
    }

    [System.Serializable]
    public class RegisterResponseData
    {
        public string accessToken;
        public string refreshToken;
        public string id;
    }

    [System.Serializable]
    private class ErrorResponse
    {
        public string type;
        public string title;
        public int status;
        public string detail;
        public string message;
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadTokens();
            Debug.Log("ApiService initialized");
        }
        else
        {
            Destroy(gameObject);
        }
    }


    public IEnumerator AutoLogin(Action<LoginResponse> onSuccess, Action<string> onError)
    {
        Debug.Log("Starting auto-login...");

        if (!HasTokens)
        {
            onError?.Invoke("No saved tokens found");
            yield break;
        }

        if (IsAccessTokenValid(accessToken))
        {
            Debug.Log("Access token is still valid, getting user info...");
            yield return GetCurrentUserInfo(onSuccess, onError);
        }
        else
        {
            Debug.Log("Access token expired, refreshing tokens...");
            yield return RefreshTokens(
                () =>
                {
                    StartCoroutine(GetCurrentUserInfo(onSuccess, onError));
                },
                error =>
                {
                    ClearTokens();
                    onError?.Invoke("Session expired. Please login again.");
                }
            );
        }
    }


    public IEnumerator Login(string email, string password, Action<LoginResponse> onSuccess, Action<string> onError)
    {
        var requestData = new LoginRequestData
        {
            username = "",
            email = email,
            password = password
        };

        string jsonData = JsonUtility.ToJson(requestData);

        using (UnityWebRequest request = UnityWebRequest.PostWwwForm($"{baseUrl}/auth/login", jsonData))
        {
            request.SetRequestHeader("Content-Type", "application/json");
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                try
                {
                    var responseData = JsonUtility.FromJson<LoginResponseData>(request.downloadHandler.text);

                    if (!string.IsNullOrEmpty(responseData.accessToken) && !string.IsNullOrEmpty(responseData.refreshToken))
                    {
                        accessToken = responseData.accessToken;
                        refreshToken = responseData.refreshToken;
                        SaveTokens();

                        var userData = DecodeJwtToken(accessToken);
                        var loginResponse = new LoginResponse
                        {
                            Token = accessToken,
                            RefreshToken = refreshToken,
                            User = new UserResponse
                            {
                                Id = userData.UserId,
                                Username = userData.Username,
                                Email = userData.Email
                            }
                        };

                        onSuccess?.Invoke(loginResponse);
                    }
                    else
                    {
                        onError?.Invoke("Invalid login response from server");
                    }
                }
                catch (Exception ex)
                {
                    onError?.Invoke($"Error parsing login response: {ex.Message}");
                }
            }
            else
            {
                onError?.Invoke(GetErrorMessage(request));
            }
        }
    }

    public IEnumerator Register(string username, string email, string password, Action<RegisterResponseData> onSuccess, Action<string> onError)
    {
        var requestData = new RegisterRequestData
        {
            username = username,
            email = email,
            password = password
        };

        string jsonData = JsonUtility.ToJson(requestData);

        using (UnityWebRequest request = UnityWebRequest.PostWwwForm($"{baseUrl}/auth/register", jsonData))
        {
            request.SetRequestHeader("Content-Type", "application/json");
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                try
                {
                    var response = JsonUtility.FromJson<RegisterResponseData>(request.downloadHandler.text);
                    onSuccess?.Invoke(response);
                }
                catch (Exception ex)
                {
                    onError?.Invoke($"Error parsing registration response: {ex.Message}");
                }
            }
            else
            {
                onError?.Invoke(GetErrorMessage(request));
            }
        }
    }


    public IEnumerator RefreshTokens(Action onSuccess, Action<string> onError)
    {
        if (isRefreshingToken)
        {
            yield return new WaitUntil(() => !isRefreshingToken);
            onSuccess?.Invoke();
            yield break;
        }

        if (string.IsNullOrEmpty(refreshToken))
        {
            onError?.Invoke("No refresh token available");
            yield break;
        }

        isRefreshingToken = true;
        Debug.Log("Refreshing tokens...");

        var requestData = new RefreshRequestData
        {
            token = refreshToken
        };

        string jsonData = JsonUtility.ToJson(requestData);

        using (UnityWebRequest request = UnityWebRequest.PostWwwForm($"{baseUrl}/auth/refresh", jsonData))
        {
            request.SetRequestHeader("Content-Type", "application/json");
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                try
                {
                    var responseData = JsonUtility.FromJson<RefreshResponseData>(request.downloadHandler.text);

                    if (!string.IsNullOrEmpty(responseData.accessToken) && !string.IsNullOrEmpty(responseData.refreshToken))
                    {
                        accessToken = responseData.accessToken;
                        refreshToken = responseData.refreshToken;
                        SaveTokens();

                        Debug.Log("Tokens refreshed successfully");
                        onSuccess?.Invoke();
                    }
                    else
                    {
                        onError?.Invoke("Invalid token response from server");
                    }
                }
                catch (Exception ex)
                {
                    onError?.Invoke($"Error parsing refresh response: {ex.Message}");
                }
            }
            else
            {
                string errorMessage = GetErrorMessage(request);
                Debug.LogError($"Token refresh failed: {errorMessage}");
                ClearTokens();
                onError?.Invoke($"Authentication failed: {errorMessage}");
            }
        }

        isRefreshingToken = false;
    }


    public IEnumerator ExecuteAuthorizedRequest(UnityWebRequest request, Action<string> onSuccess, Action<string> onError)
    {
        if (!string.IsNullOrEmpty(accessToken))
        {
            request.SetRequestHeader("Authorization", $"Bearer {accessToken}");
        }

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            onSuccess?.Invoke(request.downloadHandler.text);
        }
        else if (request.responseCode == 401)
        {
            Debug.Log("Access token expired, refreshing...");

            yield return RefreshTokens(
                () =>
                {
                    var newRequest = CreateNewRequestWithSameParameters(request);
                    newRequest.SetRequestHeader("Authorization", $"Bearer {accessToken}");
                    StartCoroutine(ExecuteRequest(newRequest, onSuccess, onError));
                },
                error =>
                {
                    onError?.Invoke("Authentication failed. Please login again.");
                    ShowLoginScreen();
                    Logout();
                }
            );
        }
        else
        {
            onError?.Invoke(GetErrorMessage(request));
        }
    }


    public IEnumerator GetLeaderboard(Action<UserLeaderboardEntryResponse[]> onSuccess, Action<string> onError)
    {
        using (UnityWebRequest request = UnityWebRequest.Get($"{baseUrl}/leaderboard/get-all-users"))
        {
            yield return ExecuteAuthorizedRequest(request,
                response =>
                {
                    try
                    {
                        var leaderboard = JsonConvert.DeserializeObject<UserLeaderboardEntryResponse[]>(request.downloadHandler.text);
                        onSuccess?.Invoke(leaderboard);
                    }
                    catch (Exception ex)
                    {
                        onError?.Invoke($"Failed to parse leaderboard: {ex.Message}");
                    }
                },
                onError
            );
        }
    }


    public IEnumerator UpdateScore(uint scoreToAdd, Action<string> onSuccess, Action<string> onError)
    {
        var userData = DecodeJwtToken(accessToken);

        var requestData = new
        {
            userId = userData.UserId,
            scoreToAdd = scoreToAdd
        };

        string jsonData = JsonUtility.ToJson(requestData);

        using (UnityWebRequest request = UnityWebRequest.PostWwwForm($"{baseUrl}/leaderboard/update", jsonData))
        {
            request.SetRequestHeader("Content-Type", "application/json");
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();

            yield return ExecuteAuthorizedRequest(request, onSuccess, onError);
        }
    }


    public IEnumerator Logout(Action onSuccess, Action<string> onError)
    {
        if (!string.IsNullOrEmpty(refreshToken))
        {
            var requestData = new
            {
                token = refreshToken
            };

            string jsonData = JsonUtility.ToJson(requestData);

            using (UnityWebRequest request = UnityWebRequest.PostWwwForm($"{baseUrl}/auth/logout", jsonData))
            {
                request.SetRequestHeader("Content-Type", "application/json");
                byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
                request.uploadHandler = new UploadHandlerRaw(bodyRaw);
                request.downloadHandler = new DownloadHandlerBuffer();

                yield return request.SendWebRequest();
            }
        }

        ClearTokens();
        onSuccess?.Invoke();
    }

    public void Logout()
    {
        ClearTokens();
        SceneManager.LoadScene("AuthTest");
    }


    private IEnumerator GetCurrentUserInfo(Action<LoginResponse> onSuccess, Action<string> onError)
    {
        if (string.IsNullOrEmpty(accessToken))
        {
            onError?.Invoke("No access token available");
            yield break;
        }

        var userData = DecodeJwtToken(accessToken);

        if (string.IsNullOrEmpty(userData.UserId))
        {
            onError?.Invoke("Invalid user data in token");
            yield break;
        }

        

        var loginResponse = new LoginResponse
        {
            Token = accessToken,
            RefreshToken = refreshToken,
            User = new UserResponse
            {
                Id = userData.UserId,
                Username = userData.Username,
                Email = userData.Email
            }
        };

        Debug.Log($"Auto-login successful for user: {userData.Username}");
        onSuccess?.Invoke(loginResponse);
    }

    private IEnumerator ExecuteRequest(UnityWebRequest request, Action<string> onSuccess, Action<string> onError)
    {
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            onSuccess?.Invoke(request.downloadHandler.text);
        }
        else
        {
            onError?.Invoke(GetErrorMessage(request));
        }
    }

    private UnityWebRequest CreateNewRequestWithSameParameters(UnityWebRequest originalRequest)
    {
        UnityWebRequest newRequest;

        switch (originalRequest.method)
        {
            case "GET":
                newRequest = UnityWebRequest.Get(originalRequest.url);
                break;
            case "POST":
                newRequest = new UnityWebRequest(originalRequest.url, "POST");
                newRequest.uploadHandler = originalRequest.uploadHandler;
                newRequest.downloadHandler = new DownloadHandlerBuffer();
                newRequest.SetRequestHeader("Content-Type", "application/json");
                break;
            case "PUT":
                newRequest = UnityWebRequest.Put(originalRequest.url, originalRequest.uploadHandler?.data);
                break;
            default:
                newRequest = UnityWebRequest.Get(originalRequest.url);
                break;
        }

        return newRequest;
    }

    private string GetErrorMessage(UnityWebRequest request)
    {
        if (!string.IsNullOrEmpty(request.downloadHandler?.text))
        {
            try
            {
                var errorResponse = JsonUtility.FromJson<ErrorResponse>(request.downloadHandler.text);
                return errorResponse.message ?? errorResponse.detail ?? request.error;
            }
            catch
            {
                return request.downloadHandler.text;
            }
        }

        return request.error ?? $"HTTP Error {request.responseCode}";
    }

    private void ShowLoginScreen()
    {
        var authController = FindObjectOfType<AuthManager>();
        if (authController != null)
        {
            authController.ShowLoginPanel();
            authController.ShowMessage("Session expired. Please login again.");
        }
    }


    public JwtUserData DecodeJwtToken(string token)
    {
        try
        {
            var parts = token.Split('.');
            if (parts.Length != 3)
                throw new ArgumentException("Invalid JWT token");

            var payload = parts[1];
            var payloadJson = DecodeBase64Url(payload);

            var jwtPayload = JsonUtility.FromJson<JwtPayload>(payloadJson);
            print(jwtPayload.nickname);
            print(jwtPayload);
            PlayerPrefs.SetString("Nickname", jwtPayload.nickname != string.Empty ? jwtPayload.nickname : "Unknown");
            return new JwtUserData
            {
                UserId = jwtPayload.nameid,
                Username = jwtPayload.nickname ?? "Unknown",
                Email = jwtPayload.email ?? "unknown@email.com",
                Exp = jwtPayload.exp
            };
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error decoding JWT token: {ex.Message}");
            return new JwtUserData();
        }
    }

    private string DecodeBase64Url(string base64Url)
    {
        var base64 = base64Url.Replace('-', '+').Replace('_', '/');
        var padding = 4 - (base64.Length % 4);
        if (padding != 4) base64 += new string('=', padding);

        var bytes = Convert.FromBase64String(base64);
        return Encoding.UTF8.GetString(bytes);
    }

    private DateTime UnixTimeStampToDateTime(long unixTimeStamp)
    {
        DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        dateTime = dateTime.AddSeconds(unixTimeStamp).ToLocalTime();
        return dateTime;
    }

    private bool IsAccessTokenValid(string token)
    {
        if (string.IsNullOrEmpty(token))
            return false;

        try
        {
            var userData = DecodeJwtToken(token);
            var expirationTime = UnixTimeStampToDateTime(userData.Exp);
            var timeUntilExpiration = expirationTime - DateTime.UtcNow;

            return timeUntilExpiration.TotalMinutes > 1;
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error validating access token: {ex.Message}");
            return false;
        }
    }


    public void SaveTokens(TokenData tokenData)
    {
        accessToken = tokenData.accessToken;
        refreshToken = tokenData.refreshToken;

        var savedData = new TokenData
        {
            accessToken = accessToken,
            refreshToken = refreshToken
        };

        string json = JsonUtility.ToJson(savedData);
        PlayerPrefs.SetString("AuthTokens", json);
        PlayerPrefs.Save();

        Debug.Log("Tokens saved successfully");
    }

    public void SaveTokens()
    {

        var savedData = new TokenData
        {
            accessToken = accessToken,
            refreshToken = refreshToken
        };

        string json = JsonUtility.ToJson(savedData);
        PlayerPrefs.SetString("AuthTokens", json);
        PlayerPrefs.Save();

        Debug.Log("Tokens saved successfully");
    }

    private void LoadTokens()
    {
        string json = PlayerPrefs.GetString("AuthTokens", "");
        if (!string.IsNullOrEmpty(json))
        {
            try
            {
                var tokenData = JsonUtility.FromJson<TokenData>(json);
                accessToken = tokenData.accessToken;
                refreshToken = tokenData.refreshToken;
                Debug.Log("Tokens loaded successfully");
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error loading tokens: {ex.Message}");
                ClearTokens();
            }
        }
    }

    public void ClearTokens()
    {
        accessToken = null;
        refreshToken = null;
        PlayerPrefs.DeleteKey("AuthTokens");
        PlayerPrefs.Save();
        Debug.Log("Tokens cleared");
    }


    public bool HasTokens => !string.IsNullOrEmpty(accessToken) && !string.IsNullOrEmpty(refreshToken);
    public bool IsLoggedIn => HasTokens;
    public string CurrentUserId => !string.IsNullOrEmpty(accessToken) ? DecodeJwtToken(accessToken).UserId : null;
}


[System.Serializable]
public class JwtUserData
{
    public string UserId;
    public string Username;
    public string Email;
    public long Exp;
}

[System.Serializable]
public class JwtPayload
{
    public string nameid;
    public string nickname;
    public string email;
    public long exp;
    public string iss;
    public string aud;
}

[System.Serializable]
public class LoginResponse
{
    public string Token;
    public string RefreshToken;
    public UserResponse User;
}

[System.Serializable]
public class UserResponse
{
    public string Id;
    public string Username;
    public string Email;
}

[System.Serializable]
public class PlayerScoreResponse
{
    public string UserId;
    public string Username;
    public uint Score;
}

[System.Serializable]
public class LeaderboardResponse
{
    public List<PlayerScoreResponse> scores;
}