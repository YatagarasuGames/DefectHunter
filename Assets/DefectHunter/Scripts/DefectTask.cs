using UnityEngine;
using Zenject;

public class DefectTask : MonoBehaviour, IInteract
{
    [SerializeField] private DefectData _defectData;
    [SerializeField] private GameObject _defectInfoObject;
    [Inject] private Canvas _playerCanvas;
    public void Interact()
    {
        var temp = Instantiate(_defectInfoObject, _playerCanvas.transform);
        temp.GetComponent<DefectInfoObject>().Init(_defectData);
    }
}
