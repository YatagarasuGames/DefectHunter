using UnityEngine;
using Zenject;

public class GameFinishListener : MonoBehaviour
{
    private int _totalDefectsOnMap;
    private int _completedDefects = 0;

    [SerializeField] private GameObject _finishGameMenu;
    [Inject] private Canvas _canvas;
    private void OnEnable()
    {
        DefectsCreator.OnDefectsCreated += Init;
        MinigameCompleted.OnMinigameCompleted += TryEndGame;
    }


    private void Init(int totalDefectsCreated)
    {
        _totalDefectsOnMap = totalDefectsCreated;
    }

    private void TryEndGame()
    {
        _completedDefects++;
        if(_completedDefects == _totalDefectsOnMap)
        {
            Instantiate(_finishGameMenu, _canvas.transform);
        }
        
    }


    private void OnDisable()
    {
        DefectsCreator.OnDefectsCreated -= Init;
        MinigameCompleted.OnMinigameCompleted -= TryEndGame;
    }
}
