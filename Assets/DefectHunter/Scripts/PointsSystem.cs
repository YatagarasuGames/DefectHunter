using UnityEngine;
using UnityEngine.Events;

public class PointsSystem : MonoBehaviour
{
    public int Points { get; private set; }
    public UnityEvent<int> OnPointsAdded;

    public void Add(int points)
    {
        Points += points;
        OnPointsAdded?.Invoke(points);
    }
}
