using System;
using System.ComponentModel.DataAnnotations;

namespace numberFightMayis.Models
{
    public class FriendMatchRequest
    {
        public int Id { get; set; }
        
        [Required]
        public string SenderId { get; set; }
        public ApplicationUser Sender { get; set; }
        
        [Required]
        public string ReceiverId { get; set; }
        public ApplicationUser Receiver { get; set; }
        
        public DateTime RequestDate { get; set; }
        public bool IsAccepted { get; set; }
        public bool IsRejected { get; set; }
        public DateTime? ResponseDate { get; set; }
    }
} 