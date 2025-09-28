using TMPro;
using UnityEngine;

public class DefectInfoObject : MonoBehaviour
{
    [SerializeField] private TMP_Text _name;
    [SerializeField] private TMP_Text _description;
    private GameObject _minigameObject;

    public void Init(DefectData defectInfo)
    {
        _name.text = defectInfo.Name;
        _description.text = defectInfo.Description;
        _minigameObject = defectInfo.Minigame;

    }


}
