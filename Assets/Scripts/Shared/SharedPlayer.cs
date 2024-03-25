using System.Collections.Generic;
using Assets.Scripts.Shared;

namespace Assets.Scripts.Server
{
    public class SharedPlayer
    {
        private readonly ushort playerId;
        private string name;
        private PlayerStates state;
        private bool? team1;

        private static readonly Dictionary<ushort, SharedPlayer> Players = new();

        public SharedPlayer(ushort playerId, string name, PlayerStates state)
        {
            this.playerId = playerId;
            this.name = name;
            this.state = state;
            Players[playerId] = this;
        }

        public static void DestroyID(ushort id)
        {
            Players.Remove(id);
        }
        
        public static SharedPlayer GetPlayerFromID(ushort id)
        {
            return Players[id];
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

        public static void ResetPlayer()
        {
            Players.Clear();
            
        }
    }
}