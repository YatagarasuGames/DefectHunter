using UnityEngine;
using Zenject;

[RequireComponent(typeof(DefectTask))]
public class DefectInteractArea : MonoBehaviour
{
    private bool _isCompleted = false;
    private DefectTask _defectTask;
    [Inject] private PlayerInteract _playerInteract;
    [SerializeField] private GameObject _exclamationMark;
    private bool _isPlayerIn = false;

    private void OnEnable()
    {
        _defectTask = GetComponent<DefectTask>();
        MinigameCompleted.OnMinigameCompleted += HandleDefectCompleted;
    }

    private void HandleDefectCompleted()
    {
        if (_isPlayerIn == false) return;
        _isCompleted = true;
        _playerInteract.gameObject.SetActive(false);
        _exclamationMark.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_isCompleted) return;
        
        if (other.gameObject.CompareTag("Player"))
        {
            _isPlayerIn = true;
            _playerInteract.gameObject.SetActive(true);
            _playerInteract.SetInteractTarget(_defectTask);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (_isCompleted) return;
        if (other.gameObject.CompareTag("Player"))
        {
            _isPlayerIn = false;
            _playerInteract.gameObject.SetActive(false);
        }
    }

    private void OnDisable()
    {
        MinigameCompleted.OnMinigameCompleted -= HandleDefectCompleted;
    }
}
