using System;

[Serializable]
public class UserData
{
    public string Username;
    public int Points;

    public UserData(string username, int points)
    {
        Username = username; Points = points;
    }
}
