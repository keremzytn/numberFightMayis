using System;

namespace numberFightMayis.Models
{
    public class DailyReward
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public DateTime ClaimDate { get; set; }
        public int Amount { get; set; }
    }
} 