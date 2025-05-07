using System;

namespace numberFightMayis.Models
{
    public class Friendship
    {
        public int Id { get; set; }
        public string RequesterId { get; set; }
        public string AddresseeId { get; set; }
        public DateTime CreatedAt { get; set; }
        public FriendshipStatus Status { get; set; }

        // Navigation properties
        public ApplicationUser Requester { get; set; }
        public ApplicationUser Addressee { get; set; }
    }

    public enum FriendshipStatus
    {
        Pending,
        Accepted,
        Rejected
    }
} 