using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class TapMinigame : Minigame
{
    [SerializeField] private TMP_Text _clickCount;
    [SerializeField] private Button _button;

    private int _currentClicks;
    [SerializeField] private int _clickGoal;

    [Inject] private PointsSystem _pointsSystem;
    

    private void OnEnable()
    {
        transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        transform.DOScale(new Vector3(1,1,1), 0.2f);
        _button.onClick.AddListener(HandleClick);
    }

    private void HandleClick()
    {
        _currentClicks++;
        _clickCount.text = _currentClicks.ToString();
        if(_currentClicks >= _clickGoal)
        {
            _pointsSystem.Add(_pointsValue);
        }
    }

    private void OnDisable()
    {
        _button.onClick.RemoveAllListeners();
    }
}
