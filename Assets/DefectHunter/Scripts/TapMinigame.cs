using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class TapMinigame : Minigame
{
    [SerializeField] private TMP_Text _clickCount;
    [SerializeField] private Button _button;
    [SerializeField] private GameObject _defectSolvedGameObject;

    private int _currentClicks;
    [SerializeField] private int _clickGoal;

    [Inject] private PointsSystem _pointsSystem;
    [Inject] private Canvas _canvas;
    

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
            
            _button.interactable = false;
            var temp = Instantiate(_defectSolvedGameObject, transform);
            temp.transform.localPosition = new Vector3(-1200, 0, 0);
            _pointsSystem.Add(_pointsValue);
        }
    }

    private void OnDisable()
    {
        _button.onClick.RemoveAllListeners();
    }
}
