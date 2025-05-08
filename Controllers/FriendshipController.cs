using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using numberFightMayis.Data;
using numberFightMayis.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace numberFightMayis.Controllers
{
    [Authorize]
    public class FriendshipController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public FriendshipController(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var friendships = await _context.Friendships
                .Include(f => f.Requester)
                .Include(f => f.Addressee)
                .Where(f => (f.RequesterId == currentUser.Id || f.AddresseeId == currentUser.Id) && f.Status == FriendshipStatus.Accepted)
                .ToListAsync();

            var friends = friendships.Select(f => 
                f.RequesterId == currentUser.Id ? f.Addressee : f.Requester
            ).ToList();

            return View(friends);
        }

        public async Task<IActionResult> FriendStats(string id)
        {
            var friend = await _userManager.FindByIdAsync(id);
            if (friend == null)
            {
                return NotFound();
            }

            var currentUser = await _userManager.GetUserAsync(User);
            var friendship = await _context.Friendships
                .FirstOrDefaultAsync(f => 
                    ((f.RequesterId == currentUser.Id && f.AddresseeId == id) ||
                     (f.RequesterId == id && f.AddresseeId == currentUser.Id)) &&
                    f.Status == FriendshipStatus.Accepted);

            if (friendship == null)
            {
                return NotFound();
            }

            return View(friend);
        }

        public async Task<IActionResult> Requests()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var requests = await _context.Friendships
                .Include(f => f.Requester)
                .Where(f => f.AddresseeId == currentUser.Id && f.Status == FriendshipStatus.Pending)
                .ToListAsync();

            return View(requests);
        }

        [HttpGet]
        public async Task<IActionResult> Search(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return Json(new { users = new object[] { } });

            var currentUser = await _userManager.GetUserAsync(User);
            var users = await _userManager.Users
                .Where(u => u.UserName.Contains(query) && u.Id != currentUser.Id)
                .Select(u => new { u.Id, u.UserName })
                .ToListAsync();

            return Json(new { users });
        }

        [HttpPost]
        public async Task<IActionResult> SendRequest(string userId)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var targetUser = await _userManager.FindByIdAsync(userId);

            if (targetUser == null)
                return NotFound();

            var existingRequest = await _context.Friendships
                .FirstOrDefaultAsync(f => 
                    (f.RequesterId == currentUser.Id && f.AddresseeId == userId) ||
                    (f.RequesterId == userId && f.AddresseeId == currentUser.Id));

            if (existingRequest != null)
                return BadRequest("Bu kullanıcıya zaten bir arkadaşlık isteği gönderilmiş.");

            var friendship = new Friendship
            {
                RequesterId = currentUser.Id,
                AddresseeId = userId,
                CreatedAt = DateTime.UtcNow,
                Status = FriendshipStatus.Pending
            };

            _context.Friendships.Add(friendship);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> AcceptRequest(int friendshipId)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var friendship = await _context.Friendships.FindAsync(friendshipId);

            if (friendship == null || friendship.AddresseeId != currentUser.Id)
                return NotFound();

            friendship.Status = FriendshipStatus.Accepted;
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> RejectRequest(int friendshipId)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var friendship = await _context.Friendships.FindAsync(friendshipId);

            if (friendship == null || friendship.AddresseeId != currentUser.Id)
                return NotFound();

            friendship.Status = FriendshipStatus.Rejected;
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> RemoveFriend(int friendshipId)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var friendship = await _context.Friendships.FindAsync(friendshipId);

            if (friendship == null || 
                (friendship.RequesterId != currentUser.Id && friendship.AddresseeId != currentUser.Id))
                return NotFound();

            _context.Friendships.Remove(friendship);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> SendFriendMatchRequest(string userId)
        {
            try
            {
                var currentUser = await _userManager.GetUserAsync(User);
                if (currentUser == null)
                {
                    return Json(new { success = false, message = "Oturum açmanız gerekiyor." });
                }

                var targetUser = await _userManager.FindByIdAsync(userId);
                if (targetUser == null)
                {
                    return Json(new { success = false, message = "Kullanıcı bulunamadı." });
                }

                // Kullanıcıların arkadaş olup olmadığını kontrol et
                var friendship = await _context.Friendships
                    .FirstOrDefaultAsync(f => 
                        (f.RequesterId == currentUser.Id && f.AddresseeId == userId) ||
                        (f.RequesterId == userId && f.AddresseeId == currentUser.Id));

                if (friendship == null || friendship.Status != FriendshipStatus.Accepted)
                {
                    return Json(new { success = false, message = "Bu kullanıcı ile arkadaş değilsiniz." });
                }

                // Aktif bir istek var mı kontrol et
                var existingRequest = await _context.FriendMatchRequests
                    .FirstOrDefaultAsync(r => 
                        r.SenderId == currentUser.Id && 
                        r.ReceiverId == userId && 
                        !r.IsAccepted && 
                        !r.IsRejected);

                if (existingRequest != null)
                {
                    return Json(new { success = false, message = "Bu kullanıcıya zaten bir dostluk maçı isteği gönderdiniz." });
                }

                var request = new FriendMatchRequest
                {
                    SenderId = currentUser.Id,
                    ReceiverId = userId,
                    RequestDate = DateTime.UtcNow,
                    IsAccepted = false,
                    IsRejected = false
                };

                _context.FriendMatchRequests.Add(request);
                await _context.SaveChangesAsync();

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                // Hata loglanabilir
                return Json(new { success = false, message = "Dostluk maçı isteği gönderilirken bir hata oluştu." });
            }
        }

        [HttpPost]
        public async Task<IActionResult> AcceptFriendMatchRequest(int requestId)
        {
            try
            {
                var currentUser = await _userManager.GetUserAsync(User);
                if (currentUser == null)
                {
                    return Json(new { success = false, message = "Oturum açmanız gerekiyor." });
                }

                var request = await _context.FriendMatchRequests
                    .Include(r => r.Sender)
                    .Include(r => r.Receiver)
                    .FirstOrDefaultAsync(r => r.Id == requestId);

                if (request == null)
                {
                    return Json(new { success = false, message = "İstek bulunamadı." });
                }

                if (request.ReceiverId != currentUser.Id)
                {
                    return Json(new { success = false, message = "Bu isteği kabul etme yetkiniz yok." });
                }

                request.IsAccepted = true;
                request.ResponseDate = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                return Json(new { success = true, redirectUrl = $"/Game/FriendMatch/{requestId}" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "İstek kabul edilirken bir hata oluştu." });
            }
        }

        [HttpPost]
        public async Task<IActionResult> RejectFriendMatchRequest(int requestId)
        {
            try
            {
                var currentUser = await _userManager.GetUserAsync(User);
                if (currentUser == null)
                {
                    return Json(new { success = false, message = "Oturum açmanız gerekiyor." });
                }

                var request = await _context.FriendMatchRequests.FindAsync(requestId);

                if (request == null)
                {
                    return Json(new { success = false, message = "İstek bulunamadı." });
                }

                if (request.ReceiverId != currentUser.Id)
                {
                    return Json(new { success = false, message = "Bu isteği reddetme yetkiniz yok." });
                }

                request.IsRejected = true;
                request.ResponseDate = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "İstek reddedilirken bir hata oluştu." });
            }
        }

        public async Task<IActionResult> PendingRequests()
        {
            try
            {
                var currentUser = await _userManager.GetUserAsync(User);
                if (currentUser == null)
                {
                    return RedirectToAction("Login", "Account");
                }
                
                var requests = await _context.FriendMatchRequests
                    .Include(r => r.Sender)
                    .Where(r => r.ReceiverId == currentUser.Id && !r.IsAccepted && !r.IsRejected)
                    .OrderByDescending(r => r.RequestDate)
                    .ToListAsync();

                return View(requests);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Home");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetPendingRequestCount()
        {
            try
            {
                var currentUser = await _userManager.GetUserAsync(User);
                if (currentUser == null)
                {
                    return Json(new { count = 0 });
                }
                
                var count = await _context.FriendMatchRequests
                    .CountAsync(r => r.ReceiverId == currentUser.Id && !r.IsAccepted && !r.IsRejected);

                return Json(new { count });
            }
            catch (Exception ex)
            {
                return Json(new { count = 0 });
            }
        }
    }
} 