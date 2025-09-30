using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

public class WallpaperPicker : MonoBehaviour
{
    [field: SerializeField] public Wallpaper WallpaperType { get; private set; }
    [SerializeField] private UnityEvent<WallpaperPicker> OnPickedWallpaperChanged;
    [SerializeField] private float _sizeModifier = 1.2f;

    public void HandleWallpaperPicked()
    {
        transform.DOScale(transform.localScale * _sizeModifier, 0.2f);
        OnPickedWallpaperChanged?.Invoke(this);
    }

    public void HandleWallpaperUnPicked()
    {
        transform.DOScale(transform.localScale / _sizeModifier, 0.2f);
        OnPickedWallpaperChanged?.Invoke(null);
    }

    public void Unpick()
    {
        transform.DOScale(transform.localScale / _sizeModifier, 0.2f);
    }
}
