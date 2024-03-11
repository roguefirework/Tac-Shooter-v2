using System.Collections;
using System.Collections.Generic;
using Shared;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    #region UI Elements
    [SerializeField] private TextMeshProUGUI usernameFields;
    [SerializeField] private TextMeshProUGUI serverIP;
    [SerializeField] private PersistentData data;
    #endregion
    #region Scenes

    [SerializeField] private string clientScene;
    [SerializeField] private string serverScene;
    #endregion
    public void StartServer()
    {
        DontDestroyOnLoad(data.gameObject);
        SceneManager.LoadScene(serverScene);
    }

    public void JoinServer()
    {
        DontDestroyOnLoad(data.gameObject);
        data.Username = usernameFields.text;
        data.TargetIP = serverIP.text;
        SceneManager.LoadScene(clientScene);
    }
}
