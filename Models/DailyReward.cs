using System;
using System.ComponentModel.DataAnnotations;

namespace numberFightMayis.Models
{
    public class DailyReward
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public DateTime ClaimDate { get; set; }
        public int Amount { get; set; }

        public virtual ApplicationUser User { get; set; }
    }
} 