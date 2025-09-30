using DG.Tweening;
using UnityEngine;

public class ExitConfirmWindow : MonoBehaviour
{
    private void OnEnable()
    {
        transform.localScale = Vector3.zero;
        transform.DOScale(new Vector3(1, 1, 1), 0.1f);
    }

    public void Exit()
    {
        DOTween.Sequence().Kill();
        Application.Quit();
    }

    public void Back()
    {
        DOTween.Sequence().Kill();
        Destroy(gameObject);
    }


}
