using Firebase.Auth;
using Firebase.Database;
using System.Collections;
using TMPro;
using UnityEngine;

public class MenuUserDataLoader : MonoBehaviour
{
    [SerializeField] private TMP_Text _username;
    [SerializeField] private TMP_Text _points;
    private void OnEnable()
    {
        _username.text = PlayerPrefs.GetString("Nickname");
    }




}
