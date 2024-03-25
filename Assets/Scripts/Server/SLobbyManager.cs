using Assets.Scripts.Shared;
using Riptide;
using Shared;
using UnityEngine;

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
                switch (args.MessageId)
                {
                    case (ushort)ClientToServerProtocol.SwitchTeam:
                        OnSwitch(args.FromConnection.Id, args.Message);
                        break;
                    case (ushort)ClientToServerProtocol.SwitchState:
                        OnSwitchReadyState(args.FromConnection.Id, args.Message);
                        break;
                    case (ushort)ClientToServerProtocol.JoinGame:
                        OnJoin(args.FromConnection.Id, args.Message);
                        break;
                }
            };
            _server.ClientDisconnected += (_, args) => OnPlayerLeave(args.Client.Id, args.Reason);
            
            this.requiredPlayers = requiredPlayers;
            this.delayTime = delayTime;
        }

        public bool CanStart()
        {
            return Time.time - startedDelay > delayTime;
        }
        
        private void OnSwitchReadyState(ushort sender, Message message)
        {
            SharedPlayer switchingPlayer = SharedPlayer.GetPlayerFromID(sender);
            
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

        private void OnPlayerLeave(ushort leavingPlayer, DisconnectReason reason)
        {
            SharedPlayer player = SharedPlayer.GetPlayerFromID(leavingPlayer);
            _lobby.RemovePlayers(player);
            Debug.Log($"[Server] player {player} left the game {reason}");
            SharedPlayer.DestroyID(leavingPlayer);
            Message toAll = Message.Create(MessageSendMode.Reliable,ServerToClientProtocol.PlayerLeave);
            toAll.AddUShort(leavingPlayer);
            _server.SendToAll(toAll);
        }
        
        private void OnJoin(ushort sender, Message message)
        {
            SharedPlayer player = new SharedPlayer(sender,message.GetString(),PlayerStates.Waiting);
            _lobby.AddPlayer(player);
            foreach (var otherPlayer in _lobby.Players())
            {
                _server.Send(CreateJoinMessageForPlayer(otherPlayer), sender);
            }
            
            _server.SendToAll(CreateJoinMessageForPlayer(player), sender);
        }

        private void OnSwitch(ushort sender, Message message)
        {
            SharedPlayer switchingPlayer = SharedPlayer.GetPlayerFromID(sender);
            
            Message toAll = Message.Create(MessageSendMode.Reliable, ServerToClientProtocol.PlayerSwitchTeam);
            toAll.AddUShort(sender);
            Debug.Assert(switchingPlayer.Team1 != null, "switchingPlayer.Team1 != null");
            switchingPlayer.Team1 = !switchingPlayer.Team1;
            toAll.AddBool((bool)switchingPlayer.Team1);
            _server.SendToAll(toAll);
        }
        
        

        private Message CreateJoinMessageForPlayer(SharedPlayer player)
        {
            Message playerJoinMessage = Message.Create(MessageSendMode.Reliable,ServerToClientProtocol.JoinPlayer);
            playerJoinMessage.AddString(player.Name);
            playerJoinMessage.AddUShort(player.PlayerId);
            playerJoinMessage.AddUShort((ushort) player.State);
            Debug.Assert(player.Team1 != null, "player.Team1 != null");
            playerJoinMessage.AddBool((bool)player.Team1);
            return playerJoinMessage;
        }
        
        
    }
}