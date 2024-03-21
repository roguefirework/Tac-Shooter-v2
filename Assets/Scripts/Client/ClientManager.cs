using System;
using System.Collections;
using System.Collections.Generic;
using Riptide;
using Shared;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ClientManager : MonoBehaviour
{
    public static ClientManager Instance { get; private set; }

    [SerializeField] private string mainMenuScene;
    [SerializeField] private ushort port;
    
    
    private PersistentData _data;
    private Client _client;
    
    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        _data = PersistentData.Instance;
        _client = new Client();
        _client.ConnectionFailed += Disconnected;
        _client.Disconnected += (_, _) => Disconnected(null,null);
        _client.Connected += Connected;
        bool connected = _client.Connect(_data.TargetIP + $":{port}",useMessageHandlers:false);
        
    }

    private void Connected(object sender, EventArgs e)
    {
        _client.Send(Message.Create(MessageSendMode.Reliable,ClientToServerProtocol.JoinGame).AddString(_data.Username));
        SceneManager.LoadScene(mainMenuScene,LoadSceneMode.Additive);
    }

    public void FixedUpdate()
    {
        _client.Update();
    }

    private void Disconnected(object sender, ConnectionFailedEventArgs connectionFailedEventArgs)
    {
        
        SceneManager.LoadScene("Main Menu",LoadSceneMode.Single);
    }

    public void OnApplicationQuit()
    {
        _client.Disconnect();
    }
}
