using System;
using System.ComponentModel.DataAnnotations;

namespace numberFightMayis.Models
{
    public class Friendship
    {
        public int Id { get; set; }
        
        [Required]
        public string RequesterId { get; set; }
        public ApplicationUser Requester { get; set; }
        
        [Required]
        public string AddresseeId { get; set; }
        public ApplicationUser Addressee { get; set; }
        
        public DateTime CreatedAt { get; set; }
        public FriendshipStatus Status { get; set; }
    }

    public enum FriendshipStatus
    {
        Pending,
        Accepted,
        Rejected
    }
} 