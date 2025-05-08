using System;
using System.Collections.Generic;

namespace numberFightMayis.Models
{
    public class Game
    {
        public int Id { get; set; }
        public int Player1Score { get; set; }
        public int Player2Score { get; set; }
        public int CurrentRound { get; set; }
        public DateTime GameStartTime { get; set; }
        public List<int> Player1UsedCards { get; set; } = new List<int>();
        public List<int> Player2UsedCards { get; set; } = new List<int>();
        public int? Player1LastCard { get; set; }
        public int? Player2LastCard { get; set; }
        public bool IsGameOver { get; set; }
        public string Winner { get; set; }
    }
} 