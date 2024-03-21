using System;
using Assets.Scripts.Server;
using Riptide;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ServerManager : MonoBehaviour
{
    
    private Server _server;
    private SLobbyManager _lobbyManager;
    [Header("Server Settings")]
    [SerializeField] private ushort port = 7777;
    [SerializeField] private ushort maxClientCount = 20;

    [Header("Game settings")] 
    [SerializeField] private int requiredPlayers;
    [SerializeField] private double delayTime;
    [Header("Level settings")] 
    [SerializeField] private string mapScene = "Level";
    public void Awake()
    {
        // Start the riptide logging
        Logging.Logging.Instance.SetupRiptideLogger();
        // Setup server
        _server = new Server();
        _server.Start(port,maxClientCount,useMessageHandlers:false);
        _server.MessageReceived += (_, arg) =>
        {
            Debug.Log(arg.MessageId);
        };
        // Load the scene
        SceneManager.LoadScene(mapScene,LoadSceneMode.Additive);
        // Open a lobby
        _lobbyManager = new SLobbyManager(_server, requiredPlayers, delayTime);
    }

    public void FixedUpdate()
    {
        _server.Update();
        if (_lobbyManager.CanStart())
        {
            
        }
    }

    public void OnApplicationQuit()
    {
        _server.Stop();
    }
}
