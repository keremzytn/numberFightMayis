using System.Collections.Generic;
using System.Threading.Tasks;
using numberFightMayis.Models;

namespace numberFightMayis.Services
{
    public interface IUserService
    {
        Task<List<ApplicationUser>> GetFriends(string userId);
        Task<bool> AddFriend(string userId, string friendId);
        Task<bool> RemoveFriend(string userId, string friendId);
    }
} 