using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using numberFightMayis.Models;
using numberFightMayis.Services;
using System;
using System.Threading.Tasks;

namespace numberFightMayis.Controllers
{
    public class GameController : Controller
    {
        private readonly GameService _gameService;
        private readonly UserManager<ApplicationUser> _userManager;
        private static Game _currentGame;

        public GameController(GameService gameService, UserManager<ApplicationUser> userManager)
        {
            _gameService = gameService;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            if (_currentGame == null || _currentGame.IsGameOver)
            {
                _currentGame = _gameService.CreateNewGame();
            }

            if (User.Identity.IsAuthenticated)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user != null)
                {
                    ViewData["UserGold"] = user.Gold;
                }
            }

            return View(_currentGame);
        }

        [HttpPost]
        public async Task<IActionResult> PlayRound(int playerCard)
        {
            if (_currentGame == null || _currentGame.IsGameOver)
            {
                return RedirectToAction("Index");
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var computerCard = _gameService.GetComputerCard(_currentGame);
            var (isValid, message) = await _gameService.PlayRound(_currentGame, playerCard, computerCard, user.Id);

            if (!isValid)
            {
                TempData["ErrorMessage"] = message;
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult NewGame()
        {
            _currentGame = _gameService.CreateNewGame();
            return RedirectToAction("Index");
        }
    }
} 