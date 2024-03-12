using System.Diagnostics;
using Assets.Scripts.Shared;
using Riptide;
using Shared;

namespace Assets.Scripts.Server
{
    public class SLobbyManager
    {
        private Riptide.Server _server;
        private Lobby _lobby;
        public SLobbyManager(Riptide.Server server, int requiredPlayers, double delayTime)
        {
            _server = server;
            _lobby = new Lobby();
            _server.MessageReceived += (_, args) =>
            {
                if (args.MessageId == (ushort)ClientToServerProtocol.JoinGame)
                    OnJoin(args.FromConnection.Id, args.Message);
            };
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

        private Message CreateJoinMessageForPlayer(SInternalPlayer player)
        {
            Message playerJoinMessage = Message.Create(MessageSendMode.Reliable,ServerToClientProtocol.JoinPlayer);
            playerJoinMessage.AddString(message.GetString());
            playerJoinMessage.AddUShort(sender);
            Debug.Assert(player.Team1 != null, "player.Team1 != null");
            playerJoinMessage.AddBool((bool)player.Team1);
            return playerJoinMessage;
        }
        
        
    }
}