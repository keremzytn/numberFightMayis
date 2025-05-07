using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace numberFightMayis.Models
{
    public class ApplicationUser : IdentityUser
    {
        public int Gold { get; set; } = 500; // Yeni kayıt olan kullanıcılara 500 gold başlangıç değeri
        
        // Arkadaşlık ilişkileri
        public ICollection<Friendship> SentFriendRequests { get; set; }
        public ICollection<Friendship> ReceivedFriendRequests { get; set; }
    }
}