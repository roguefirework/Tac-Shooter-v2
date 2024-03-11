using System.Collections.Generic;

namespace Assets.Scripts.Server
{
    public class Lobby
    {
        List<SInternalPlayer> playersInLobby = new();
        List<SInternalPlayer> team1 = new();
        List<SInternalPlayer> team2 = new();
        public Lobby()
        {
            
        }


        public void AddPlayer(SInternalPlayer player)
        {
            if (team1.Count > team2.Count)
            {
                team2.Add(player);
            }
            else
            {
                team1.Add(player);
            }
            playersInLobby.Add(player);
        }

        public void RemovePlayers(SInternalPlayer player)
        {
            if (team1.Contains(player))
            {
                team1.Remove(player);
            }
            else
            {
                team2.Remove(player);
            }
            playersInLobby.Remove(player);
        }
        
        public IReadOnlyList<SInternalPlayer> Team1()
        {
            return team1;
        }
        
        public IReadOnlyList<SInternalPlayer> Team2()
        {
            return team2;
        }
    }
}