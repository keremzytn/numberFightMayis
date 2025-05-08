using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace numberFightMayis.Models
{
    public class Friendship
    {
        public int Id { get; set; }
        
        [Required]
        public string RequesterId { get; set; } = string.Empty;
        
        [Required]
        public string AddresseeId { get; set; } = string.Empty;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public FriendshipStatus Status { get; set; }

        [ForeignKey("RequesterId")]
        public virtual ApplicationUser? Requester { get; set; }
        
        [ForeignKey("AddresseeId")]
        public virtual ApplicationUser? Addressee { get; set; }
    }

    public enum FriendshipStatus
    {
        Pending,
        Accepted,
        Rejected
    }
} 