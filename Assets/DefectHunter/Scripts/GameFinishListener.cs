using UnityEngine;
using Zenject;

public class GameFinishListener : MonoBehaviour
{
    private int _totalDefectsOnMap;
    private int _completedDefects = 0;

    [SerializeField] private GameObject _finishGameMenu;
    [Inject] private Canvas _canvas;
    [Inject] private DiContainer _di;
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
            
            var menu = Instantiate(_finishGameMenu, _canvas.transform);
            _di.Inject(menu.GetComponent<GameEndMenu>());
        }
        
    }


    private void OnDisable()
    {
        DefectsCreator.OnDefectsCreated -= Init;
        MinigameCompleted.OnMinigameCompleted -= TryEndGame;
    }
}
