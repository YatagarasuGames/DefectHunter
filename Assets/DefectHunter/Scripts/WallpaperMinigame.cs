using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Zenject;


public enum Wallpaper
{
    None = 0,
    Green = 1,
    Blue = 2,
    Yellow = 3,

}

[Serializable]
public struct WallpaperType
{
    public Wallpaper Type;
    public Sprite Ui;
}
public class WallpaperMinigame : Minigame
{
    [SerializeField] private List<WallpaperPicker> _wallpapersPickers;
    [SerializeField] private List<WallpaperUIInMinigame> _wallpapers;
    [SerializeField] private List<WallpaperType> _wallpaperTypes;
    [SerializeField] private GameObject _defectSolvedGameObject;

    private int _completedWallpapers = 0;
    [Inject] private PointsSystem _pointsSystem;
    public Wallpaper PickedWallpaper { get; private set; } = Wallpaper.None;
    private WallpaperPicker _currentSelectedWallpaper;
    private void OnEnable()
    {
        transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        transform.DOScale(new Vector3(1, 1, 1), 0.2f);

        foreach (var  wallpaper in _wallpapers)
        {
            wallpaper.Init(_wallpaperTypes[UnityEngine.Random.Range(0, _wallpaperTypes.Count)]);
        }
    }

    public void HandlePickWallpaper(WallpaperPicker picker)
    {

        if(picker == null) { PickedWallpaper = Wallpaper.None; return; }
        if (_currentSelectedWallpaper != null) _currentSelectedWallpaper.Unpick();
        PickedWallpaper = picker.WallpaperType;
        _currentSelectedWallpaper = picker;
    }

    public void HandlePaintWallpaper()
    {
        _completedWallpapers++;

        if (_completedWallpapers == _wallpapers.Count)
        {
            var temp = Instantiate(_defectSolvedGameObject, transform);
            temp.transform.localPosition = new Vector3(-1200, 0, 0);
            _pointsSystem.Add(_pointsValue);
        }

    }

    private void OnDisable()
    {
        DOTween.Sequence().Kill();
    }


}
