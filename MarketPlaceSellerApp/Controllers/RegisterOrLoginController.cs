using MarketPlaceSellerApp.HashingPassword;
using MarketPlaceSellerApp.Helpers;
using MarketPlaceSellerApp.Models;
using MarketPlaceSellerApp.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MarketPlaceSellerApp.Controllers
{
	[Route("[controller]")]
	[ApiController]
	public class RegisterAndLoginApiController : ControllerBase
	{
		public readonly HepsiburadaSellerInformationContext _context;
		private readonly AuthHelpers _authHelpers;
		private readonly IWebHostEnvironment _webHostEnvironment;

		public RegisterAndLoginApiController(HepsiburadaSellerInformationContext context, AuthHelpers authHelpers, IWebHostEnvironment webHostEnvironment)
		{
			_context = context;
			_authHelpers = authHelpers;
			_webHostEnvironment = webHostEnvironment;
		}

		[HttpPost("RegisterUser")]
		public async Task<IActionResult> RegisterUser([FromForm] User model)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(new { Success = false, ErrorMessage = "Geçersiz model girdisi." });
			}

			try
			{
				// Kullanıcı adı veya email'in daha önce kullanılıp kullanılmadığını kontrol et
				var existingUser = await _context.UserData
					.FirstOrDefaultAsync(m => m.UserName == model.UserName || m.Email == model.Email);

				if (existingUser != null)
				{
					var errorMessage = existingUser.UserName == model.UserName
						? "Kullanıcı adı zaten mevcut."
						: "Email adresi zaten kayıtlı.";
					return BadRequest(new { Success = false, ErrorMessage = errorMessage });
				}

				// Şifreyi hashle
				string hashPassword = HashingAndVerifyPassword.HashingPassword.HashPassword(model.Password);

				// Profil resmi kaydetme
				string profileImagePath = null;

				if (model.ProfileImage != null)
				{
					profileImagePath = await SaveProfileImageAsync(model.ProfileImage);
				}

				// Yeni kullanıcı oluştur
				var user = new UserDatum
				{
					FirstName = model.FirstName,
					LastName = model.LastName,
					UserName = model.UserName,
					Email = model.Email,
					Password = hashPassword,
					ProfileImage = profileImagePath
				};

				// Yaş bilgisi varsa kaydet
				if (!string.IsNullOrWhiteSpace(model.Age))
				{
					user.Age = Convert.ToDateTime(model.Age);
				}

				// Kullanıcıyı veritabanına ekle ve kaydet
				_context.UserData.Add(user);
				await _context.SaveChangesAsync();

				return Ok(new { Success = true, Message = "Kullanıcı başarıyla kaydedildi." });
			}
			catch (Exception ex)
			{
				// Hata oluştuğunda 500 döndür
				return StatusCode(500, new { Success = false, ErrorMessage = $"Sunucu hatası: {ex.Message}" });
			}
		}


		// Profil resmini kaydet
		private async Task<string> SaveProfileImageAsync(IFormFile profileImageBytes)
		{
			try
			{
				// Dosya adı oluştur
				var fileName = $"{Guid.NewGuid().ToString("N")}.jpg";
				var filePath = Path.Combine(_webHostEnvironment.WebRootPath, "profile_images");

				// Klasör yoksa oluştur
				Directory.CreateDirectory(filePath);

				// Dosya yolunu belirle
				var fullFilePath = Path.Combine(filePath, fileName);

				// Dosyayı yaz
				using (var fileStream = new FileStream(fullFilePath, FileMode.Create))
				{
					await profileImageBytes.CopyToAsync(fileStream);
				}
				// Sadece dosya adını döndür
				return fileName;
			}
			catch (Exception ex)
			{
				throw new Exception($"Profil resmi yüklenirken hata oluştu: {ex.Message}");
			}
		}

		[HttpPost("LoginUserData")]
		public async Task<IActionResult> LoginUserData([FromBody] LoginUser model)
		{
			if (model == null)
			{
				return BadRequest(new { Success = false, ErrorMessage = "Boş Değer Gönderilemez" });
			}

			try
			{
				var authResult = await _authHelpers.UserAuthentication(model);
				return authResult;
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { Success = false, ErrorMessage = $"Sunucu hatası: {ex.Message}" });
			}
		}
	}
}
