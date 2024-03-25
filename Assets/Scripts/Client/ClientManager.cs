using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Server;
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
    public Client Client;
    public SharedPlayer player;
    
    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        Logging.Logging.Instance.SetupRiptideLogger();
        _data = PersistentData.Instance;
        
        Client = new Client();
        Client.ConnectionFailed += Disconnected;
        Client.Disconnected += (_, _) => Disconnected(null,null);
        Client.Connected += Connected;
        
        string ip = _data.TargetIP + ':' + port;
        Client.Connect(ip,useMessageHandlers:false);
    }

    private void Connected(object sender, EventArgs e)
    {
        Client.Send(Message.Create(MessageSendMode.Reliable,ClientToServerProtocol.JoinGame).AddString(_data.Username));
        SceneManager.LoadScene(mainMenuScene,LoadSceneMode.Additive);
    }

    public void FixedUpdate()
    {
        Client.Update();
    }

    private void Disconnected(object sender, ConnectionFailedEventArgs connectionFailedEventArgs)
    {
        SharedPlayer.ResetPlayer();
        SceneManager.LoadScene("Main Menu",LoadSceneMode.Single);
    }

    public void OnApplicationQuit()
    {
        Client.Disconnect();
    }
}
