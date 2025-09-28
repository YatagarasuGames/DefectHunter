using DG.Tweening;
using TMPro;
using UnityEngine;
using Zenject;

public class UIPoints : MonoBehaviour
{
    [Inject] private PointsSystem _pointsSystem;
    [SerializeField] private TMP_Text _pointsText;
    [SerializeField] private TMP_Text _pointsAddedEffect;

    [SerializeField] private int visualEffectsOffset = -100;
    [SerializeField] private float visualEffectsDuration = 2;
    private void OnEnable()
    {
        _pointsSystem.OnPointsAdded.AddListener(HandlePointsAdd);
    }

    private void HandlePointsAdd(int pointsAdded)
    {
        _pointsText.text = _pointsSystem.Points.ToString();
        CreateVisualEffects(pointsAdded);
    }

    private void CreateVisualEffects(int pointsAdded)
    {
        var temp = Instantiate(_pointsAddedEffect, transform);
        temp.text = pointsAdded.ToString();
        temp.transform.localPosition = new Vector3(0, visualEffectsOffset, 0);
        temp.transform.DOLocalMoveY(0, visualEffectsDuration);
        temp.DOColor(new Color(255, 255, 255, 0), visualEffectsDuration).OnComplete(
            () =>
            {
                Destroy(temp);
            }
            );
    }

    private void OnDisable()
    {
        _pointsSystem.OnPointsAdded.RemoveListener(HandlePointsAdd);
    }
}
