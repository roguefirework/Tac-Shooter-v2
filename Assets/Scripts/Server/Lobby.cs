using System.Collections.Generic;

namespace Assets.Scripts.Server
{
    public class Lobby
    {
        readonly List<SharedPlayer> playersInLobby = new();
        readonly List<SharedPlayer> team1 = new();
        readonly List<SharedPlayer> team2 = new();
        public Lobby()
        {
            
        }


        public void AddPlayer(SharedPlayer player)
        {
            bool? team = player.Team1;
            if (team != null)
            {
                if ((bool)team) 
                {
                    team1.Add(player);
                }
                else
                {
                    team2.Add(player);
                }
            }
            else
            {
                if (team1.Count > team2.Count)
                {
                    team2.Add(player);
                    player.Team1 = false;
                }
                else
                {
                    team1.Add(player);
                    player.Team1 = true;
                }

                playersInLobby.Add(player);
            }
        }

        public void RemovePlayers(SharedPlayer player)
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

        public void SwapTeam(SharedPlayer player)
        {
            if (team1.Contains(player))
            {
                team1.Remove(player);
                team2.Add(player);
            }
            else
            {
                team2.Remove(player);
                team1.Add(player);
            }
            player.Team1 = !player.Team1;
        }
        
        public IReadOnlyList<SharedPlayer> Team1()
        {
            return team1;
        }
        
        public IReadOnlyList<SharedPlayer> Team2()
        {
            return team2;
        }

        public IReadOnlyList<SharedPlayer> Players()
        {
            return playersInLobby;
        }
    }
}