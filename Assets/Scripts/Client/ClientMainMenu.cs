using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Server;
using Assets.Scripts.Shared;
using Riptide;
using Shared;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class ClientMainMenu : MonoBehaviour
{
    [FormerlySerializedAs("team1obj")] [SerializeField]
    private Transform team1Obj;
    [FormerlySerializedAs("team2obj")] [SerializeField]
    private Transform team2Obj;

    [SerializeField] private Color readyColor;
    [SerializeField] private Color notReadyColor;
    [SerializeField] private Color playingColor;
    [SerializeField] private Color notJoinedColor;
    
    private Button[] _team1;

    private Button[] _team2;

    private Lobby _lobby;
    // Start is called before the first frame update
    void Start()
    {
        _team1 = new Button[5];
        _team2 = new Button[5];
        for (int i = 0; i < team1Obj.childCount; i++)
        {
            _team1[i] = team1Obj.GetChild(i).gameObject.GetComponent<Button>();
            _team1[i].onClick.AddListener(() => ClickTeam(true));
        }
        
        for (int i = 0; i < team2Obj.childCount; i++)
        {
            _team2[i] = team2Obj.GetChild(i).gameObject.GetComponent<Button>();
            _team2[i].onClick.AddListener(() => ClickTeam(false));
        }
        
        _lobby = new Lobby();
        
        ClientManager.Instance.Client.MessageReceived += (_, args) =>
        {
            switch (args.MessageId)
            {
                case (ushort)ServerToClientProtocol.JoinPlayer:
                    OnJoin(args.Message);
                    break;
                case (ushort)ServerToClientProtocol.PlayerSwitchState:
                    OnSwitchState(args.Message);
                    break;
                case (ushort)ServerToClientProtocol.PlayerSwitchTeam:
                    OnSwitchTeam(args.Message);
                    break;
            }
        };
    }

    private void ClickTeam(bool team1)
    {
        if (team1 != ClientManager.Instance.player.Team1)
        {
            Message toServer = Message.Create(MessageSendMode.Reliable, ClientToServerProtocol.SwitchTeam);
            ClientManager.Instance.Client.Send(toServer);
        }
    }
    
    public void ClickReady()
    {
        Message toServer = Message.Create(MessageSendMode.Reliable, ClientToServerProtocol.SwitchState);
        ClientManager.Instance.Client.Send(toServer);
    }
    
    private void Update()
    {
        void AddPlayer(Button button, SharedPlayer sInternalPlayer)
        {
            button.GetComponent<Image>().color = sInternalPlayer.State switch
            {
                PlayerStates.Ready => readyColor,
                PlayerStates.Waiting => notReadyColor,
                _ => playingColor
            };
            button.transform.GetChild(0).GetComponent<TMP_Text>().SetText(sInternalPlayer.Name);
        }
        void AddNotPlayer(Button button)
        {
            button.GetComponent<Image>().color = notJoinedColor;
            button.transform.GetChild(0).GetComponent<TMP_Text>().SetText("Join");
        }
        
        for (var i = 0; i < _lobby.Team1().Count; i++)
        {
            Button button = _team1[i];
            SharedPlayer player = _lobby.Team1()[i];
            AddPlayer(button, player);
        }

        for (var i = _lobby.Team1().Count; i < 5; i++)
        {
            Button button = _team1[i];
            AddNotPlayer(button);
        }
        for (var i = 0; i < _lobby.Team2().Count; i++)
        {
            Button button = _team2[i];
            SharedPlayer player = _lobby.Team2()[i];
            AddPlayer(button, player);
        }
        for (var i = _lobby.Team2().Count; i < 5; i++)
        {
            Button button = _team2[i];
            AddNotPlayer(button);
        }
    }




    private void OnJoin(Message argsMessage)
    {
        
        string playerName = argsMessage.GetString();
        ushort id = argsMessage.GetUShort();

        PlayerStates state = (PlayerStates) argsMessage.GetUShort();
        bool team = argsMessage.GetBool();
        SharedPlayer player = new SharedPlayer(id, playerName, state);
        player.Team1 = team;
        _lobby.AddPlayer(player);
        if (PersistentData.Instance.Username == playerName)
        {
            ClientManager.Instance.player = SharedPlayer.GetPlayerFromID(id);
        }
    }

    private void OnSwitchTeam(Message args)
    {
        ushort sender = args.GetUShort();
        bool team = args.GetBool();
        if (SharedPlayer.GetPlayerFromID(sender).Team1 != team)
        {
            _lobby.SwapTeam(SharedPlayer.GetPlayerFromID(sender));
        }
    }

    private void OnSwitchState(Message args)
    {
        ushort sender = args.GetUShort();
        PlayerStates state = (PlayerStates) args.GetUShort();
        SharedPlayer.GetPlayerFromID(sender).State = state;
    }

    private void OnDisconnect(Message args)
    {
        SharedPlayer player = SharedPlayer.GetPlayerFromID(args.GetUShort());
        _lobby.RemovePlayers(player);
        Debug.Log($"[Server] player {player.Name} left the game");
        SharedPlayer.DestroyID(player.PlayerId);
    }
}
