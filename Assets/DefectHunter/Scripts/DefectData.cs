using UnityEngine;

[CreateAssetMenu(fileName = "DefectData", menuName = "NewDefectData", order = 51)]
public class DefectData : ScriptableObject
{
    [field: SerializeField] public string Name { get; private set; }
    [field: SerializeField] public string Description { get; private set; }
    [field: SerializeField] public GameObject Minigame { get; private set; }
}
