using System.Collections.Generic;
using System.Collections.ObjectModel;
using Assets.Scripts.Server;

namespace Shared
{
    public enum RoundPhase
    {
        Buy,
        Active,
        BombPlanted,
        Post
    }
    
    public class Game
    {
        public static readonly ReadOnlyDictionary<RoundPhase, float> roundTimes = new ReadOnlyDictionary<RoundPhase,float>(
            new Dictionary<RoundPhase, float>()
            {
                {RoundPhase.Buy, 30},
                {RoundPhase.Active, 180 },
                {RoundPhase.BombPlanted, 60},
                { RoundPhase.Post, 10}
            });

        public List<SharedPlayer> players;
        public Timer roundTimer;
    }
}