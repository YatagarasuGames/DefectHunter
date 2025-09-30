using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class DefectsCreator : MonoBehaviour
{
    [SerializeField] private List<DefectData> _defectsData;
    [SerializeField] private List<Transform> _defectsSpawnPoints;
    [SerializeField] private int _defectCount;
    [SerializeField] private GameObject _defectArea;
    [Inject] private DiContainer _di;

    public static System.Action<int> OnDefectsCreated;
    private void OnEnable()
    {
        CreateDefectAreas();
    }

    private void CreateDefectAreas()
    {
        if (_defectCount > _defectsSpawnPoints.Count) _defectCount = _defectsSpawnPoints.Count;

        for (int i = 0; i < _defectCount; i++)
        {
            var tempPosition = _defectsSpawnPoints[Random.Range(0, _defectsSpawnPoints.Count)];
            var defectData = _defectsData[Random.Range(0, _defectsData.Count)];
            var defectArea = Instantiate(_defectArea);
            defectArea.transform.position = tempPosition.transform.position;
            defectArea.GetComponent<DefectTask>().Init(defectData);
            _di.Inject(defectArea.GetComponent<DefectInteractArea>());
            _di.Inject(defectArea.GetComponent<DefectTask>());
            _defectsSpawnPoints.Remove(tempPosition);
        }
        OnDefectsCreated?.Invoke(_defectCount);
    }
}
