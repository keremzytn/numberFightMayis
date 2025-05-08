using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace numberFightMayis.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? ProfileImageUrl { get; set; }
        public int Gold { get; set; }
        public int TotalGames { get; set; }
        public int Wins { get; set; }
        public int Losses { get; set; }
        public int Draws { get; set; }

        public ICollection<Friendship> SentFriendRequests { get; set; }
        public ICollection<Friendship> ReceivedFriendRequests { get; set; }
        public ICollection<FriendMatchRequest> SentMatchRequests { get; set; }
        public ICollection<FriendMatchRequest> ReceivedMatchRequests { get; set; }
    }
}