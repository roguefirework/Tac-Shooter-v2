
using System;
using Riptide;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ServerManager : MonoBehaviour
{
    
    private Server _server;
    [Header("Server Settings")]
    [SerializeField] private ushort port = 7777;
    [SerializeField] private ushort maxClientCount = 20;

    [Header("Level settings")] 
    [SerializeField] private string mapScene = "Level";
    public void Awake()
    {
        // Start the riptide logging
        Logging.Logging.Instance.SetupRiptideLogger();
        // Setup server
        _server = new Server();
        _server.Start(port,maxClientCount);
        // Load the scene
        SceneManager.LoadScene(mapScene,LoadSceneMode.Additive);
    }

    public void OnApplicationQuit()
    {
        _server.Stop();
    }
}
