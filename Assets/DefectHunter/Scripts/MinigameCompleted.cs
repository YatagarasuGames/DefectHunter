using DG.Tweening;
using System;
using UnityEngine;

public class MinigameCompleted : MonoBehaviour
{
    public static Action OnMinigameCompleted;
    private void OnEnable()
    {
        transform.DOLocalMoveX(0, 0.3f);
        OnMinigameCompleted?.Invoke();
    }

    private void Init(int pointsEarned)
    {

    }

    public void Back()
    {
        transform.parent.DOScale(transform.parent.localScale * 1.2f, 0.2f).OnComplete(
            () =>
            {
                transform.parent.DOScale(Vector3.zero, 0.3f).OnComplete(
                () => 
                {
                    Destroy(transform.parent.gameObject); 
                }
                );
            } 
        );
    }
}
