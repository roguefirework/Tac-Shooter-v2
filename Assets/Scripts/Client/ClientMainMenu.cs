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
using UnityEngine.UI;

public class ClientMainMenu : MonoBehaviour
{
    [SerializeField]
    private Transform team1obj;
    [SerializeField]
    private Transform team2obj;

    [SerializeField] private Color readyColor;
    [SerializeField] private Color notReadyColor;
    [SerializeField] private Color playingColor;
    [SerializeField] private Color notJoinedColor;
    
    private Button[] team1;

    private Button[] team2;

    private Lobby lobby;
    // Start is called before the first frame update
    void Start()
    {
        team1 = new Button[5];
        team2 = new Button[5];
        for (int i = 0; i < team1obj.childCount; i++)
        {
            team1[i] = team1obj.GetChild(i).gameObject.GetComponent<Button>();
            team1[i].onClick.AddListener(() => ClickTeam(true));
        }
        
        for (int i = 0; i < team2obj.childCount; i++)
        {
            team2[i] = team2obj.GetChild(i).gameObject.GetComponent<Button>();
            team2[i].onClick.AddListener(() => ClickTeam(false));
        }
        
        lobby = new Lobby();
        
        ClientManager.Instance.Client.MessageReceived += (_, args) =>
        {
            if (args.MessageId == (ushort)ServerToClientProtocol.JoinPlayer)
                OnJoin(args.Message);
        };
        ClientManager.Instance.Client.MessageReceived += (_, args) =>
        {
            if (args.MessageId == (ushort)ServerToClientProtocol.PlayerSwitchState)
                OnSwitchState(args.Message);
        };
        ClientManager.Instance.Client.MessageReceived += (_, args) =>
        {
            if (args.MessageId == (ushort)ServerToClientProtocol.PlayerSwitchTeam)
                OnSwitchTeam(args.Message);
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
        void AddPlayer(Button button, SInternalPlayer sInternalPlayer)
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
        
        for (var i = 0; i < lobby.Team1().Count; i++)
        {
            Button button = team1[i];
            SInternalPlayer player = lobby.Team1()[i];
            AddPlayer(button, player);
        }

        for (var i = lobby.Team1().Count; i < 5; i++)
        {
            Button button = team1[i];
            AddNotPlayer(button);
        }
        for (var i = 0; i < lobby.Team2().Count; i++)
        {
            Button button = team2[i];
            SInternalPlayer player = lobby.Team2()[i];
            AddPlayer(button, player);
        }
        for (var i = lobby.Team2().Count; i < 5; i++)
        {
            Button button = team2[i];
            AddNotPlayer(button);
        }
    }




    private void OnJoin(Message argsMessage)
    {
        
        string playerName = argsMessage.GetString();
        ushort id = argsMessage.GetUShort();

        PlayerStates state = (PlayerStates) argsMessage.GetUShort();
        bool team = argsMessage.GetBool();
        SInternalPlayer player = new SInternalPlayer(id, playerName, state);
        player.Team1 = team;
        lobby.AddPlayer(player);
        if (PersistentData.Instance.Username == playerName)
        {
            ClientManager.Instance.player = SInternalPlayer.GetPlayerFromID(id);
        }
    }

    private void OnSwitchTeam(Message args)
    {
        ushort sender = args.GetUShort();
        bool team = args.GetBool();
        if (SInternalPlayer.GetPlayerFromID(sender).Team1 != team)
        {
            lobby.SwapTeam(SInternalPlayer.GetPlayerFromID(sender));
        }
    }

    private void OnSwitchState(Message args)
    {
        ushort sender = args.GetUShort();
        PlayerStates state = (PlayerStates) args.GetUShort();
        SInternalPlayer.GetPlayerFromID(sender).State = state;
    }
}
