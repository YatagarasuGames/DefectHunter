using UnityEngine;
using Zenject;
public class DefectInteractArea : MonoBehaviour
{
    private bool _isCompleted = false;
    [Inject] private PlayerInteract _playerInteract;
    private void OnTriggerEnter(Collider other)
    {
        if (_isCompleted) return;
        if (other.gameObject.CompareTag("Player"))
        {
            _playerInteract.gameObject.SetActive(true);
            //_playerInteract.SetInteractTarget()
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
