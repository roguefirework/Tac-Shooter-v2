using System.Collections.Generic;
using Assets.Scripts.Shared;

namespace Assets.Scripts.Server
{
    public class SInternalPlayer
    {
        private readonly uint playerId;
        private string name;
        private PlayerStates state;
        static Dictionary<uint, SInternalPlayer> _players;

        public SInternalPlayer(uint playerId, string name, PlayerStates state)
        {
            this.playerId = playerId;
            this.name = name;
            this.state = state;
            _players[playerId] = this;
        }

        public static SInternalPlayer GetPlayerFromID(uint id)
        {
            return _players[id];
        }
        
        public uint PlayerId => playerId;

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
        
    }
}