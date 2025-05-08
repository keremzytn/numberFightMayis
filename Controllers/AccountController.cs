using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using numberFightMayis.Models;
using numberFightMayis.ViewModels;
using System.Threading.Tasks;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace numberFightMayis.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public AccountController(
            UserManager<ApplicationUser> userManager, 
            SignInManager<ApplicationUser> signInManager,
            IWebHostEnvironment webHostEnvironment)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return Redirect("/");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }
                    return RedirectToAction("Index", "Home");
                }
                ModelState.AddModelError(string.Empty, "Geçersiz giriş denemesi.");
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Redirect("http://localhost:5206/");
        }

        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login");
            }

            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateUsername(string newUsername)
        {
            if (string.IsNullOrWhiteSpace(newUsername))
            {
                TempData["Error"] = "Kullanıcı adı boş olamaz.";
                return RedirectToAction("Profile");
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login");
            }

            // Kullanıcı adının benzersiz olup olmadığını kontrol et
            var existingUser = await _userManager.FindByNameAsync(newUsername);
            if (existingUser != null && existingUser.Id != user.Id)
            {
                TempData["Error"] = "Bu kullanıcı adı zaten kullanılıyor.";
                return RedirectToAction("Profile");
            }

            user.UserName = newUsername;
            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                TempData["Success"] = "Kullanıcı adı başarıyla güncellendi.";
            }
            else
            {
                TempData["Error"] = "Kullanıcı adı güncellenirken bir hata oluştu.";
            }

            return RedirectToAction("Profile");
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(string currentPassword, string newPassword, string confirmPassword)
        {
            if (string.IsNullOrWhiteSpace(currentPassword) || string.IsNullOrWhiteSpace(newPassword) || string.IsNullOrWhiteSpace(confirmPassword))
            {
                TempData["Error"] = "Tüm alanları doldurunuz.";
                return RedirectToAction("Profile");
            }

            if (newPassword != confirmPassword)
            {
                TempData["Error"] = "Yeni şifreler eşleşmiyor.";
                return RedirectToAction("Profile");
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login");
            }

            var result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);

            if (result.Succeeded)
            {
                TempData["Success"] = "Şifreniz başarıyla değiştirildi.";
            }
            else
            {
                TempData["Error"] = "Şifre değiştirilirken bir hata oluştu. Lütfen mevcut şifrenizi kontrol ediniz.";
            }

            return RedirectToAction("Profile");
        }

        [HttpPost]
        public async Task<IActionResult> UpdateProfileImage(IFormFile profileImage)
        {
            if (profileImage == null || profileImage.Length == 0)
            {
                TempData["Error"] = "Lütfen bir resim seçin.";
                return RedirectToAction("Profile");
            }

            // Dosya uzantısını kontrol et
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
            var fileExtension = Path.GetExtension(profileImage.FileName).ToLowerInvariant();
            if (!allowedExtensions.Contains(fileExtension))
            {
                TempData["Error"] = "Sadece JPG, JPEG, PNG ve WEBP formatları desteklenmektedir.";
                return RedirectToAction("Profile");
            }

            // Dosya boyutunu kontrol et (max 5MB)
            if (profileImage.Length > 5 * 1024 * 1024)
            {
                TempData["Error"] = "Dosya boyutu 5MB'dan küçük olmalıdır.";
                return RedirectToAction("Profile");
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login");
            }

            // Benzersiz dosya adı oluştur
            var fileName = $"{user.Id}_{DateTime.UtcNow.Ticks}{fileExtension}";
            var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "profile-images");
            
            // Klasör yoksa oluştur
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            var filePath = Path.Combine(uploadsFolder, fileName);

            // Eski profil resmini sil (varsayılan değilse)
            if (!string.IsNullOrEmpty(user.ProfileImageUrl) && user.ProfileImageUrl != "/img/avatar.webp")
            {
                var oldFilePath = Path.Combine(_webHostEnvironment.WebRootPath, user.ProfileImageUrl.TrimStart('/'));
                if (System.IO.File.Exists(oldFilePath))
                {
                    System.IO.File.Delete(oldFilePath);
                }
            }

            // Yeni resmi kaydet
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await profileImage.CopyToAsync(stream);
            }

            // Kullanıcı profilini güncelle
            user.ProfileImageUrl = $"/uploads/profile-images/{fileName}";
            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                TempData["Success"] = "Profil resminiz başarıyla güncellendi.";
            }
            else
            {
                TempData["Error"] = "Profil resmi güncellenirken bir hata oluştu.";
            }

            return RedirectToAction("Profile");
        }
    }
}