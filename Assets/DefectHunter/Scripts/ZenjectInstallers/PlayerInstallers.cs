using UnityEngine;
using Zenject;

public class PlayerInstallers : MonoInstaller
{
    [SerializeField] private PlayerInteract _playerInteract;
    public override void InstallBindings()
    {
        Container.Bind<PlayerInteract>().FromInstance(_playerInteract).AsSingle();
    }
}