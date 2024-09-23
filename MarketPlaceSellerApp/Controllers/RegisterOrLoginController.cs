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
	public class RegisterOrLoginApiController : ControllerBase
	{
		private readonly AuthHelpers _authHelpers;
		private readonly IWebHostEnvironment _webHostEnvironment;

		public RegisterOrLoginApiController(AuthHelpers authHelpers, IWebHostEnvironment webHostEnvironment)
		{
			_authHelpers = authHelpers;
			_webHostEnvironment = webHostEnvironment;
		}

		[HttpPost("RegisterUser")]
		public async Task<IActionResult> RegisterUser([FromForm] User model, IFormFile profileImage)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(new { Success = false, ErrorMessage = "Geçersiz model girdisi." });
			}

			try
			{
				var existingUser = await _authHelpers._context.UserData
					.AsNoTracking()
					.FirstOrDefaultAsync(m => m.UserName == model.UserName || m.Email == model.Email);

				if (existingUser != null)
				{
					if (existingUser.UserName == model.UserName)
					{
						return BadRequest(new { Success = false, ErrorMessage = "Kullanıcı adı zaten mevcut." });
					}
					if (existingUser.Email == model.Email)
					{
						return BadRequest(new { Success = false, ErrorMessage = "Email adresi zaten kayıtlı." });
					}
				}

				string hashPassword = HashingAndVerifyPassword.HashingPassword.HashPassword(model.Password);

				string profileImagePath = null;

				if (profileImage != null && profileImage.Length > 0)
				{
					var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(profileImage.FileName);

					var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "profile_images");
					if (!Directory.Exists(uploadsFolder))
					{
						Directory.CreateDirectory(uploadsFolder);
					}

					profileImagePath = Path.Combine(uploadsFolder, uniqueFileName);

					using (var fileStream = new FileStream(profileImagePath, FileMode.Create))
					{
						await profileImage.CopyToAsync(fileStream);
					}

					profileImagePath = Path.Combine("uploads", "profile_images", uniqueFileName);
				}

				var user = new UserDatum
				{
					FirstName = model.FirstName,
					LastName = model.LastName,
					UserName = model.UserName,
					Email = model.Email,
					Age = model.Age,
					Password = hashPassword,
					ProfileImage = profileImagePath
				};

				await _authHelpers._context.UserData.AddAsync(user);
				await _authHelpers._context.SaveChangesAsync();

				return Ok(new { Success = true, Message = "Kullanıcı başarıyla kaydedildi." });
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { Success = false, ErrorMessage = $"Sunucu hatası: {ex.Message}" });
			}
		}



		[HttpPost("LoginUserData")]
		public async Task<IActionResult> LoginUserData([FromBody] LoginUser model)
		{
			if (model == null)
			{
				return BadRequest(new { Success = false, Message = "Boş Değer Gönderilemez" });
			}
			try
			{
				var authResult = await _authHelpers.UserAuthentication(model);
				if (authResult is OkObjectResult)
				{
					return authResult;
				}
				else
				{
					return authResult;
				}
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { Success = false, ErrorMessage = ex.Message });
			}
		}
	}
}

