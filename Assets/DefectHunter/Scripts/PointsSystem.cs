using UnityEngine;
using UnityEngine.Events;

public class PointsSystem : MonoBehaviour
{
    private string Points = "0";
    public UnityEvent<int> OnPointsAdded;
    private AES _aes;
    private void OnEnable()
    {
        _aes = new AES((Time.time + Random.Range(0, 100)).ToString());
    }

    public void Add(int points)
    {
        if (Points == "0")
        {
            Points = points.ToString();

            Points = _aes.Encrypt(Points.ToString());
        }
        else
        {
            int intPoints = int.Parse((_aes.Decrypt(Points)));
            
            intPoints += points;
            Points = intPoints.ToString();

            Points = _aes.Encrypt(Points.ToString());
        }
        OnPointsAdded?.Invoke(points);
    }

    public int GetDecryptedPoints()
    {
        return int.Parse((_aes.Decrypt(Points)));
    }
}
