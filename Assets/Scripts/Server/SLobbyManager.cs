using Assets.Scripts.Shared;
using Riptide;
using Shared;
using UnityEngine;
using Debug = System.Diagnostics.Debug;

namespace Assets.Scripts.Server
{
    public class SLobbyManager
    {
        private Riptide.Server _server;
        private Lobby _lobby;
        private int requiredPlayers;
        private double delayTime;
        private int readyPlayers;
        
        
        private double startedDelay;
        
        public SLobbyManager(Riptide.Server server, int requiredPlayers, double delayTime)
        {
            _server = server;
            _lobby = new Lobby();
            _server.MessageReceived += (_, args) =>
            {
                if (args.MessageId == (ushort)ClientToServerProtocol.JoinGame)
                    OnJoin(args.FromConnection.Id, args.Message);
            };
            _server.MessageReceived += (_, args) =>
            {
                if (args.MessageId == (ushort)ClientToServerProtocol.SwitchTeam)
                    OnSwitch(args.FromConnection.Id, args.Message);
            };
            _server.MessageReceived += (_, args) =>
            {
                if (args.MessageId == (ushort)ClientToServerProtocol.SwitchState)
                    OnSwitchReadyState(args.FromConnection.Id, args.Message);
            };
            
            
            this.requiredPlayers = requiredPlayers;
            this.delayTime = delayTime;
        }

        public bool CanStart()
        {
            return Time.time - startedDelay > delayTime;
        }
        
        private void OnSwitchReadyState(ushort sender, Message message)
        {
            SInternalPlayer switchingPlayer = SInternalPlayer.GetPlayerFromID(sender);
            
            Message toAll = Message.Create(MessageSendMode.Reliable, ServerToClientProtocol.PlayerSwitchState);
            toAll.AddUShort(sender);
            Debug.Assert(switchingPlayer.Team1 != null, "switchingPlayer.Team1 != null");
            switchingPlayer.State = switchingPlayer.State == PlayerStates.Waiting ? PlayerStates.Ready : PlayerStates.Waiting;
            if (switchingPlayer.State == PlayerStates.Ready)
            {
                readyPlayers++;
            }
            else
            {
                readyPlayers--;
            }

            startedDelay = readyPlayers > requiredPlayers ? Time.time : 10000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000.0; // (never)
            
            toAll.AddUShort((ushort)switchingPlayer.State);
            _server.SendToAll(toAll);
        }
        
        private void OnJoin(ushort sender, Message message)
        {
            SInternalPlayer player = new SInternalPlayer(sender,message.GetString(),PlayerStates.Waiting);
            _lobby.AddPlayer(player);
            foreach (var otherPlayer in _lobby.Players())
            {
                _server.Send(CreateJoinMessageForPlayer(otherPlayer), sender);
            }
            
            _server.SendToAll(CreateJoinMessageForPlayer(player), sender);
        }

        private void OnSwitch(ushort sender, Message message)
        {
            SInternalPlayer switchingPlayer = SInternalPlayer.GetPlayerFromID(sender);
            
            Message toAll = Message.Create(MessageSendMode.Reliable, ServerToClientProtocol.PlayerSwitchTeam);
            toAll.AddUShort(sender);
            Debug.Assert(switchingPlayer.Team1 != null, "switchingPlayer.Team1 != null");
            switchingPlayer.Team1 = !switchingPlayer.Team1;
            toAll.AddBool((bool)switchingPlayer.Team1);
            _server.SendToAll(toAll);
        }
        
        

        private Message CreateJoinMessageForPlayer(SInternalPlayer player)
        {
            Message playerJoinMessage = Message.Create(MessageSendMode.Reliable,ServerToClientProtocol.JoinPlayer);
            playerJoinMessage.AddString(player.Name);
            playerJoinMessage.AddUShort(player.PlayerId);
            Debug.Assert(player.Team1 != null, "player.Team1 != null");
            playerJoinMessage.AddBool((bool)player.Team1);
            return playerJoinMessage;
        }
        
        
    }
}