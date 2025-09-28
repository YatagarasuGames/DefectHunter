using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class TapMinigame : MonoBehaviour
{
    [SerializeField] private TMP_Text _clickCount;
    [SerializeField] private Button _button;

    private int _currentClicks;
    [SerializeField] private int _clickGoal;
    [SerializeField] private int _pointsValue;

    [Inject] private PointsSystem _pointsSystem;
    

    private void OnEnable()
    {
        
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
