using System;
using System.Collections.Generic;
using System.Linq;
using numberFightMayis.Models;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace numberFightMayis.Services
{
    public class GameService
    {
        private readonly Random _random = new Random();
        private readonly UserManager<ApplicationUser> _userManager;

        public GameService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public Game CreateNewGame()
        {
            return new Game
            {
                Player1Score = 0,
                Player2Score = 0,
                CurrentRound = 1,
                GameStartTime = DateTime.Now,
                IsGameOver = false
            };
        }

        private bool HasConsecutiveThreeNumbers(List<int> numbers)
        {
            if (numbers.Count < 3) return false;

            var sortedNumbers = numbers.OrderBy(n => n).ToList();
            for (int i = 0; i <= sortedNumbers.Count - 3; i++)
            {
                if (sortedNumbers[i + 1] == sortedNumbers[i] + 1 && 
                    sortedNumbers[i + 2] == sortedNumbers[i] + 2)
                {
                    return true;
                }
            }
            return false;
        }

        public bool IsValidCard(Game game, int card, bool isPlayer1)
        {
            var usedCards = isPlayer1 ? game.Player1UsedCards : game.Player2UsedCards;
            var lastCard = isPlayer1 ? game.Player1LastCard : game.Player2LastCard;

            // Kural 1: Kullanılmış kartı tekrar kullanamaz
            if (usedCards.Contains(card))
                return false;

            // Kural 2: Önceki kartın bir eksiği ve bir fazlası kullanılamaz
            if (lastCard.HasValue)
            {
                if (card == lastCard.Value - 1 || card == lastCard.Value + 1)
                    return false;
            }

            // Kural 3: 5. raundda ardışık 3 sayı varsa ve ortadaki sayı kullanılıyorsa
            // bir sonraki raundda bir eksiği ve bir fazlası etkisizleştirilmeyecek
            if (game.CurrentRound == 5)
            {
                var remainingCards = Enumerable.Range(1, 7)
                    .Where(c => !usedCards.Contains(c))
                    .ToList();

                if (HasConsecutiveThreeNumbers(remainingCards))
                {
                    var middleCard = remainingCards.OrderBy(n => n).Skip(1).First();
                    if (card == middleCard)
                    {
                        return true;
                    }
                }
            }

            return true;
        }

        public int GetComputerCard(Game game)
        {
            var availableCards = Enumerable.Range(1, 7)
                .Where(card => IsValidCard(game, card, false))
                .ToList();

            if (!availableCards.Any())
                return 0;

            // Basit bir strateji: Rastgele bir kart seç
            return availableCards[_random.Next(availableCards.Count)];
        }

        private async Task UpdateUserStats(ApplicationUser user, bool isWin, bool isDraw)
        {
            user.TotalGames++;
            if (isWin)
                user.Wins++;
            else if (isDraw)
                user.Draws++;
            else
                user.Losses++;

            await _userManager.UpdateAsync(user);
        }

        public async Task<(bool isValid, string message)> PlayRound(Game game, int player1Card, int player2Card, string playerId)
        {
            if (game.IsGameOver)
                return (false, "Oyun zaten bitmiş durumda.");

            if (game.CurrentRound > 7)
                return (false, "Maksimum raund sayısına ulaşıldı.");

            if (!IsValidCard(game, player1Card, true))
                return (false, "Geçersiz kart seçimi.");

            if (!IsValidCard(game, player2Card, false))
                return (false, "Bilgisayar geçersiz kart seçti.");

            // Kartları kaydet
            game.Player1UsedCards.Add(player1Card);
            game.Player2UsedCards.Add(player2Card);
            game.Player1LastCard = player1Card;
            game.Player2LastCard = player2Card;

            // Puanları güncelle
            if (player1Card > player2Card)
                game.Player1Score++;
            else if (player2Card > player1Card)
                game.Player2Score++;

            game.CurrentRound++;

            // Oyun bitti mi kontrol et
            if (game.CurrentRound > 7)
            {
                game.IsGameOver = true;
                var user = await _userManager.FindByIdAsync(playerId);
                if (user != null)
                {
                    if (game.Player1Score > game.Player2Score)
                    {
                        game.Winner = "Oyuncu 1";
                        user.Gold += 100;
                        await UpdateUserStats(user, true, false);
                    }
                    else if (game.Player2Score > game.Player1Score)
                    {
                        game.Winner = "Bilgisayar";
                        await UpdateUserStats(user, false, false);
                    }
                    else
                    {
                        game.Winner = "Berabere";
                        await UpdateUserStats(user, false, true);
                    }
                }
            }

            return (true, "Raund başarıyla tamamlandı.");
        }
    }
} 