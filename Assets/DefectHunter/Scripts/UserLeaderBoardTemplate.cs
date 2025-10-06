using TMPro;
using UnityEngine;

public class UserLeaderBoardTemplate : MonoBehaviour
{
    [SerializeField] private TMP_Text _numberInLeaderboard;
    [SerializeField] private TMP_Text _username;
    [SerializeField] private TMP_Text _points;
    public void Init(int numberInLeaderboard, string username, int points)
    {
        _numberInLeaderboard.text = numberInLeaderboard.ToString();
        _username.text = username;
        _points.text = points.ToString();
    }
}
