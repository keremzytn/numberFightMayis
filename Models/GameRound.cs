using System;

namespace numberFightMayis.Models
{
    public class GameRound
    {
        public int Id { get; set; }
        public string GameId { get; set; } = string.Empty;
        public int RoundNumber { get; set; }
        public int? Player1Number { get; set; }
        public int? Player2Number { get; set; }
        public string? Winner { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public virtual Game? Game { get; set; }
    }
} 