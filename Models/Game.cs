using System;
using System.Collections.Generic;

namespace numberFightMayis.Models
{
    public class Game
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Player1Id { get; set; } = string.Empty;
        public string Player2Id { get; set; } = string.Empty;
        public int Player1Score { get; set; }
        public int Player2Score { get; set; }
        public GameStatus Status { get; set; }
        public string? Winner { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsGameOver { get; set; }
        public int CurrentRound { get; set; }
        public List<int> Player1UsedCards { get; set; } = new List<int>();
        public List<int> Player2UsedCards { get; set; } = new List<int>();
        public int? Player1LastCard { get; set; }
        public int? Player2LastCard { get; set; }

        public virtual ApplicationUser? Player1 { get; set; }
        public virtual ApplicationUser? Player2 { get; set; }
        public virtual ICollection<GameRound> Rounds { get; set; } = new List<GameRound>();
    }

    public enum GameStatus
    {
        WaitingForPlayer1,
        InProgress,
        Finished
    }
} 