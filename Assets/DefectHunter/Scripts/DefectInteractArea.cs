using UnityEngine;
using Zenject;

[RequireComponent(typeof(DefectTask))]
public class DefectInteractArea : MonoBehaviour
{
    private bool _isCompleted = false;
    [Inject] private PlayerInteract _playerInteract;
    [Inject] private DiContainer _container;
    private DefectTask _defectTask;

    private void Awake()
    {
        _defectTask = GetComponent<DefectTask>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (_isCompleted) return;
        if (other.gameObject.CompareTag("Player"))
        {
            _playerInteract.gameObject.SetActive(true);
            _playerInteract.SetInteractTarget(_defectTask);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (_isCompleted) return;
        if (other.gameObject.CompareTag("Player"))
        {
            _playerInteract.gameObject.SetActive(false);
        }
    }
}
