using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Server;
using Assets.Scripts.Shared;
using Riptide;
using Shared;
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
    
    private Transform[] team1;

    private Transform[] team2;

    private Lobby lobby;
    // Start is called before the first frame update
    void Start()
    {
        team1 = new Transform[5];
        team2 = new Transform[5];
        for (int i = 0; i < team1obj.childCount; i++)
        {
            team1[i] = transform.GetChild(0).GetChild(i);
            team1[i].GetComponent<Button>();
        }
        for (int i = 0; i < team2obj.childCount; i++)
        {
            team2[i] = transform.GetChild(0).GetChild(i);
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
    
    
    
    
    
    private void OnJoin(Message argsMessage)
    {
        string playerName = argsMessage.GetString();
        ushort id = argsMessage.GetUShort();
        PlayerStates state = (PlayerStates) argsMessage.GetUShort();
        bool team = argsMessage.GetBool();
        SInternalPlayer player = new SInternalPlayer(id, playerName, state);
        player.Team1 = team;
        lobby.AddPlayer(player);
    }

    private void OnSwitchTeam(Message args)
    {
        ushort sender = args.GetUShort();
        bool team = args.GetBool();
        SInternalPlayer.GetPlayerFromID(sender).Team1 = team;
    }

    private void OnSwitchState(Message args)
    {
        ushort sender = args.GetUShort();
        PlayerStates state = (PlayerStates) args.GetUShort();
        SInternalPlayer.GetPlayerFromID(sender).State = state;
    }
}
