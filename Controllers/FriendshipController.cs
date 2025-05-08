using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using numberFightMayis.Data;
using numberFightMayis.Models;
using System;
using System.Collections.Generic;
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

        [HttpPost]
        public async Task<IActionResult> SendRequest(string userId)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null) return NotFound();

            var targetUser = await _userManager.FindByIdAsync(userId);
            if (targetUser == null) return NotFound();

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
            if (currentUser == null) return NotFound();

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
            if (currentUser == null) return NotFound();

            var friendship = await _context.Friendships.FindAsync(friendshipId);
            if (friendship == null || friendship.AddresseeId != currentUser.Id)
                return NotFound();

            _context.Friendships.Remove(friendship);
            await _context.SaveChangesAsync();

            return Ok();
        }

        public async Task<IActionResult> Requests()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null) return NotFound();

            var requests = await _context.Friendships
                .Include(f => f.Requester)
                .Where(f => f.AddresseeId == currentUser.Id && f.Status == FriendshipStatus.Pending)
                .ToListAsync();

            return View(requests);
        }

        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null) return NotFound();

            var friendships = await _context.Friendships
                .Include(f => f.Requester)
                .Include(f => f.Addressee)
                .Where(f => (f.RequesterId == currentUser.Id || f.AddresseeId == currentUser.Id) && f.Status == FriendshipStatus.Accepted)
                .ToListAsync();

            var friends = friendships.Select(f => f.RequesterId == currentUser.Id ? f.Addressee : f.Requester).ToList();
            return View(friends);
        }

        public async Task<IActionResult> FriendStats(string id)
        {
            var friend = await _userManager.FindByIdAsync(id);
            if (friend == null) return NotFound();

            return View(friend);
        }

        [HttpGet]
        public async Task<IActionResult> Search(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return Json(new { users = new List<object>() });

            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null) return NotFound();

            var users = await _userManager.Users
                .Where(u => u.UserName.Contains(query) && u.Id != currentUser.Id)
                .Select(u => new { u.Id, u.UserName })
                .ToListAsync();

            return Json(new { users });
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
    }
} 