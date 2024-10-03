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
		public async Task<IActionResult> RegisterUser([FromBody] User model) 
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(new { Success = false, ErrorMessage = "Geçersiz model girdisi." });
			}

			try
			{
				var existingUser = await _context.UserData
					.FirstOrDefaultAsync(m => m.UserName == model.UserName || m.Email == model.Email);

				if (existingUser != null)
				{
					var errorMessage = existingUser.UserName == model.UserName
						? "Kullanıcı adı zaten mevcut."
						: "Email adresi zaten kayıtlı.";
					return BadRequest(new { Success = false, ErrorMessage = errorMessage });
				}

				string hashPassword = HashingAndVerifyPassword.HashingPassword.HashPassword(model.Password);

				string profileImagePath = null;

				if (!string.IsNullOrWhiteSpace(model.ProfileImage))
				{
					var profileImageBytes = Convert.FromBase64String(model.ProfileImage);
					profileImagePath = await SaveProfileImageAsync(profileImageBytes); 
				}

				var user = new UserDatum
				{
					FirstName = model.FirstName,
					LastName = model.LastName,
					UserName = model.UserName,
					Email = model.Email,
					Password = hashPassword,
					ProfileImage = profileImagePath
				};

				if (!string.IsNullOrWhiteSpace(model.Age))
				{
					user.Age = Convert.ToDateTime(model.Age);
				}

				_context.UserData.Add(user);
				await _context.SaveChangesAsync();

				return Ok(new { Success = true, Message = "Kullanıcı başarıyla kaydedildi." });
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { Success = false, ErrorMessage = $"Sunucu hatası: {ex.Message}" });
			}
		}

		private async Task<string> SaveProfileImageAsync(byte[] profileImageBytes)
		{
			try
			{
				var fileName = $"{Guid.NewGuid().ToString("N")}.jpg";
				var filePath = Path.Combine(_webHostEnvironment.ContentRootPath, "wwwroot", "profile_images");

				Directory.CreateDirectory(filePath);

				var fullFilePath = Path.Combine(filePath, fileName);

				await System.IO.File.WriteAllBytesAsync(fullFilePath, profileImageBytes);

				return Path.Combine(fileName);
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
