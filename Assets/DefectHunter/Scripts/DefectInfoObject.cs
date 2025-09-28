using TMPro;
using UnityEngine;
using Zenject;

public class DefectInfoObject : MonoBehaviour
{
    [SerializeField] private TMP_Text _name;
    [SerializeField] private TMP_Text _description;
    private GameObject _minigameObject;
    private DiContainer _diContainer;

    public void Init(DefectData defectInfo, DiContainer diContainer)
    {
        _name.text = defectInfo.Name;
        _description.text = defectInfo.Description;
        _minigameObject = defectInfo.Minigame;
        _diContainer = diContainer;

    }

    public void CreateMinigame()
    {
        var temp = Instantiate(_minigameObject, transform.parent);
        _diContainer.Inject(temp.GetComponent<Minigame>());
    }
    public void Back()
    {
        Destroy(gameObject);
    }


}
