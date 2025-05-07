using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using numberFightMayis.Services;

namespace numberFightMayis.Controllers
{
    [Authorize]
    public class DailyRewardController : Controller
    {
        private readonly DailyRewardService _dailyRewardService;

        public DailyRewardController(DailyRewardService dailyRewardService)
        {
            _dailyRewardService = dailyRewardService;
        }

        [HttpPost]
        public async Task<IActionResult> ClaimDailyReward()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var success = await _dailyRewardService.ClaimDailyReward(userId);
            
            if (success)
                return Json(new { success = true, message = "Günlük ödülünüz başarıyla alındı! 100 gold kazandınız." });
            else
                return Json(new { success = false, message = "Günlük ödülünüzü zaten aldınız. Lütfen 24 saat sonra tekrar deneyin." });
        }
    }
} 