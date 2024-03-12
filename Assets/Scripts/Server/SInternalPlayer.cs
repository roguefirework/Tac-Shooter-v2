using System.Collections.Generic;
using Assets.Scripts.Shared;

namespace Assets.Scripts.Server
{
    public class SInternalPlayer
    {
        private readonly ushort playerId;
        private string name;
        private PlayerStates state;
        private bool? team1;
        
        static Dictionary<ushort, SInternalPlayer> _players;

        public SInternalPlayer(ushort playerId, string name, PlayerStates state)
        {
            this.playerId = playerId;
            this.name = name;
            this.state = state;
            _players[playerId] = this;
        }

        public static void DestroyID(ushort id)
        {
            _players.Remove(id);
        }
        
        public static SInternalPlayer GetPlayerFromID(ushort id)
        {
            return _players[id];
        }
        
        public ushort PlayerId => playerId;

        public string Name
        {
            get => name;
            set => name = value;
        }

        public PlayerStates State
        {
            get => state;
            set => state = value;
        }

        public bool? Team1
        {
            get => team1;
            set => team1 = value;
        }
        
    }
}