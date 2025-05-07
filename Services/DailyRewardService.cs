using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using numberFightMayis.Models;
using Microsoft.EntityFrameworkCore;
using numberFightMayis.Data;

namespace numberFightMayis.Services
{
    public class DailyRewardService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public DailyRewardService(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public async Task<bool> ClaimDailyReward(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return false;

            // Kullanıcının son ödül alma zamanını kontrol et
            var lastRewardClaim = await _context.DailyRewards
                .Where(dr => dr.UserId == userId)
                .OrderByDescending(dr => dr.ClaimDate)
                .FirstOrDefaultAsync();

            // Eğer son ödül 24 saatten önce alınmışsa veya hiç alınmamışsa
            if (lastRewardClaim == null || 
                (DateTime.UtcNow - lastRewardClaim.ClaimDate).TotalHours >= 24)
            {
                // Kullanıcıya 100 gold ekle
                user.Gold += 100;

                // Ödül kaydını oluştur
                var newReward = new DailyReward
                {
                    UserId = userId,
                    ClaimDate = DateTime.UtcNow,
                    Amount = 100
                };

                _context.DailyRewards.Add(newReward);
                await _context.SaveChangesAsync();
                await _userManager.UpdateAsync(user);

                return true;
            }

            return false;
        }
    }
} 