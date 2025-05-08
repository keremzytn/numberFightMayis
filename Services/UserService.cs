using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using numberFightMayis.Models;
using Microsoft.EntityFrameworkCore;
using numberFightMayis.Data;
using System.Linq;

namespace numberFightMayis.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public UserService(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public async Task<List<ApplicationUser>> GetFriends(string userId)
        {
            var friendships = await _context.Friendships
                .Where(f => (f.RequesterId == userId || f.AddresseeId == userId) && 
                           f.Status == FriendshipStatus.Accepted)
                .ToListAsync();

            var friendIds = friendships
                .Select(f => f.RequesterId == userId ? f.AddresseeId : f.RequesterId)
                .ToList();

            return await _userManager.Users
                .Where(u => friendIds.Contains(u.Id))
                .ToListAsync();
        }

        public async Task<bool> AddFriend(string userId, string friendId)
        {
            if (userId == friendId)
                return false;

            var existingFriendship = await _context.Friendships
                .FirstOrDefaultAsync(f => 
                    (f.RequesterId == userId && f.AddresseeId == friendId) ||
                    (f.RequesterId == friendId && f.AddresseeId == userId));

            if (existingFriendship != null)
                return false;

            var friendship = new Friendship
            {
                RequesterId = userId,
                AddresseeId = friendId,
                Status = FriendshipStatus.Pending
            };

            _context.Friendships.Add(friendship);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RemoveFriend(string userId, string friendId)
        {
            var friendship = await _context.Friendships
                .FirstOrDefaultAsync(f => 
                    (f.RequesterId == userId && f.AddresseeId == friendId) ||
                    (f.RequesterId == friendId && f.AddresseeId == userId));

            if (friendship == null)
                return false;

            _context.Friendships.Remove(friendship);
            await _context.SaveChangesAsync();
            return true;
        }
        
    }
} 