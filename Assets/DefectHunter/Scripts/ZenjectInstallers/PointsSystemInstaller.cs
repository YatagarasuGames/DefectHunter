using System.Runtime.CompilerServices;
using UnityEngine;
using Zenject;

public class PointsSystemInstaller : MonoInstaller
{
    [SerializeField] private PointsSystem _pointsSystem;
    public override void InstallBindings()
    {
        Container.Bind<PointsSystem>().FromInstance(_pointsSystem).AsSingle();
    }
}