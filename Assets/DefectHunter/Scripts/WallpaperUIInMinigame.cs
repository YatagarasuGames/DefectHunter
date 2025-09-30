using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class WallpaperUIInMinigame : MonoBehaviour
{
    [SerializeField] private Wallpaper _wallpaperType;
    [SerializeField] private Image _wallpaperUI;
    [SerializeField] private WallpaperMinigame _wallpaperMinigame;
    public UnityEvent OnComplete;

    private bool _isCompleted = false;
    public void Init(WallpaperType type)
    {
        _wallpaperType = type.Type;
        _wallpaperUI.sprite = type.Ui;
        _wallpaperUI.color = new Color(_wallpaperUI.color.r, _wallpaperUI.color.g, _wallpaperUI.color.b, 0.5f);
    }

    public void HandleCompleteAttempt()
    {
        if (_isCompleted) return;
        if(_wallpaperMinigame.PickedWallpaper == _wallpaperType)
        {
            OnComplete?.Invoke();
            _isCompleted = true;
            _wallpaperUI.color = new Color(_wallpaperUI.color.r, _wallpaperUI.color.g, _wallpaperUI.color.b, 1);
        }
    }
}
