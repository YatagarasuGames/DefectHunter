using UnityEngine;
using Zenject;

public class PlayerInstallers : MonoInstaller
{
    [SerializeField] private PlayerInteract _playerInteract;
    [SerializeField] private Canvas _playerCavas;
    public override void InstallBindings()
    {
        Container.Bind<PlayerInteract>().FromInstance(_playerInteract).AsSingle();
        Container.Bind<Canvas>().FromInstance(_playerCavas).AsSingle();
    }
}