using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using ReservationSystem.Models;
using System.IO;
using Microsoft.AspNetCore.Http;

namespace ReservationSystem.Areas.Identity.Pages.Account.Manage
{
    public class IndexModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, ILogger<IndexModel> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public string Username { get; set; }

        public class InputModel
        {
            [Phone]
            [Display(Name = "Telefon Numarası")]
            public string PhoneNumber { get; set; }

            [Display(Name = "Ad Soyad")]
            public string FullName { get; set; }

            [Display(Name = "Doğum Tarihi")]
            [DataType(DataType.Date)]
            public DateTime? BirthDate { get; set; }

            [Display(Name = "Cinsiyet")]
            public string Gender { get; set; }

            [Display(Name = "İl")]
            public string City { get; set; }

            [Display(Name = "İlçe")]
            public string District { get; set; }

            [Display(Name = "Adres")]
            public string Address { get; set; }

            [Display(Name = "Profil Fotoğrafı")]
            public string? PhotoPath { get; set; }
        }

        private async Task LoadAsync(ApplicationUser user)
        {
            Username = await _userManager.GetUserNameAsync(user);
            Input = new InputModel
            {
                PhoneNumber = await _userManager.GetPhoneNumberAsync(user),
                FullName = user.FullName,
                BirthDate = user.BirthDate,
                Gender = user.Gender,
                City = user.City,
                District = user.District,
                Address = user.Address,
                PhotoPath = user.PhotoPath
            };
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");

            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");

            if (!ModelState.IsValid)
            {
                await LoadAsync(user); 
                return Page();
            }

            _logger.LogInformation("Formdan gelen veriler: FullName={FullName}, BirthDate={BirthDate}, Gender={Gender}, City={City}, District={District}, Address={Address}",
                Input.FullName, Input.BirthDate, Input.Gender, Input.City, Input.District, Input.Address);

            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            if (Input.PhoneNumber != phoneNumber)
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
                if (!setPhoneResult.Succeeded)
                {
                    StatusMessage = "Telefon numarası güncellenemedi.";
                    await LoadAsync(user);
                    return Page();
                }
            }

            user.FullName = Input.FullName;
            user.BirthDate = Input.BirthDate;
            user.Gender = Input.Gender;
            user.City = Input.City;
            user.District = Input.District;
            user.Address = Input.Address;

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                foreach (var error in updateResult.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);

                await LoadAsync(user);
                return Page();
            }

            await _signInManager.RefreshSignInAsync(user);

            StatusMessage = "Profil bilgileriniz güncellendi.";

            await LoadAsync(user); 
            return Page();
        }
        public async Task<IActionResult> OnPostUploadPhotoAsync(IFormFile photoFile)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");

            if (photoFile != null && photoFile.Length > 0)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(photoFile.FileName);
                var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");

                if (!Directory.Exists(uploadsPath))
                    Directory.CreateDirectory(uploadsPath);

                var filePath = Path.Combine(uploadsPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await photoFile.CopyToAsync(stream);
                }

                user.PhotoPath = "/uploads/" + fileName;
                var updateResult = await _userManager.UpdateAsync(user);

                if (!updateResult.Succeeded)
                {
                    foreach (var error in updateResult.Errors)
                        ModelState.AddModelError(string.Empty, error.Description);

                    await LoadAsync(user);
                    return Page();
                }

                StatusMessage = "Fotoğraf başarıyla yüklendi.";
            }
            else
            {
                StatusMessage = "Geçerli bir fotoğraf seçilmedi.";
            }

            await LoadAsync(user);
            return RedirectToPage();
        }

    }
}
