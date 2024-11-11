using MarketPlaceSellerApp.HashingPassword;
using MarketPlaceSellerApp.Models;
using MarketPlaceSellerApp.ViewModel;
using MarketPlaceSellerApp.FileNameGuid;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MarketPlaceSellerApp.Controllers
{
	[Authorize]
	[Route("[controller]")]
	[ApiController]
	public class UserUpdateApiController : ControllerBase
	{
		private readonly HepsiburadaSellerInformationContext _context;
		private readonly GuidOperation _guidOperation;

		public UserUpdateApiController(HepsiburadaSellerInformationContext context, GuidOperation guidOperation)
		{
			_context = context;
			_guidOperation = guidOperation;
		}

		[HttpPost("UpdatePassword")]
		public async Task<IActionResult> UpdatePassword([FromBody] UpdatePassword model)
		{
			if (string.IsNullOrEmpty(model.UserName) || string.IsNullOrEmpty(model.Password))
			{
				return BadRequest(new
				{
					Success = false,
					ErrorMessage = "Kullanıcı adı ve şifre alanları zorunludur.",
					UserGuidance = "Lütfen kullanıcı adı ve yeni şifre bilgilerini girin."
				});
			}

			try
			{
				var user = await _context.UserData.AsNoTracking().FirstOrDefaultAsync(m => m.UserName == model.UserName);

				if (user != null)
				{
					string hashPassword = HashingAndVerifyPassword.HashingPassword.HashPassword(model.Password);
					user.Password = hashPassword;

					_context.UserData.Update(user);
					await _context.SaveChangesAsync();
					return Ok(new
					{
						Success = true,
						Message = "Şifre başarıyla güncellendi.",
						UserGuidance = "Güncellenen şifrenizle giriş yapabilirsiniz."
					});
				}
				else
				{
					return BadRequest(new
					{
						Success = false,
						ErrorMessage = "Kullanıcı bulunamadı.",
						UserGuidance = "Geçerli bir kullanıcı adı girin ve tekrar deneyin."
					});
				}
			}
			catch (Exception ex)
			{
				return StatusCode(500, new
				{
					Success = false,
					ErrorMessage = "Sunucu ile iletişimde bir hata oluştu. Lütfen tekrar deneyin.",
					Details = ex.Message,
					UserGuidance = "Şifre güncelleme işlemi sırasında bir hata oluştu. Tekrar deneyin veya destek ekibine başvurun."
				});
			}
		}

		[HttpPost("EditUserData")]
		public async Task<IActionResult> UpdateUserData([FromBody] UpdateUserData model)
		{
			if (string.IsNullOrEmpty(model.UserName))
			{
				return BadRequest(new
				{
					Success = false,
					ErrorMessage = "Kullanıcı adı zorunludur.",
					UserGuidance = "Lütfen geçerli bir kullanıcı adı girin."
				});
			}

			try
			{
				var user = await _context.UserData.FirstOrDefaultAsync(m => m.UserName == model.UserName);

				if (user != null)
				{
					user.FirstName = model.FirstName;
					user.LastName = model.LastName;
					user.Email = model.Email;
					user.Age = model.Age;

					if (!string.IsNullOrWhiteSpace(model.ProfileImageBase64) && !model.ProfileImageBase64.StartsWith("http", StringComparison.OrdinalIgnoreCase))
					{
						var profileImagePath = await _guidOperation.SaveProfileImageAsync(model.ProfileImageBase64);
						user.ProfileImage = profileImagePath;
					}

					await _context.SaveChangesAsync();

					var data = await _context.UserData
						.Where(u => u.UserName == model.UserName)
						.Select(u => new UpdateUserData
						{
							FirstName = u.FirstName,
							LastName = u.LastName,
							UserName = u.UserName,
							Email = u.Email,
							Age = u.Age.Value,
							ProfileImageBase64 = string.IsNullOrEmpty(u.ProfileImage)
								? null
								: $"https://0686-37-130-115-91.ngrok-free.app/profile_images/{u.ProfileImage}"
						})
						.FirstOrDefaultAsync();

					return Ok(new ProfileUpdateApiResponse
					{
						Success = true,
						ErrorMessage = "Kullanıcı bilgileri başarıyla güncellendi.",
						Data = data,
					});
				}
				else
				{
					return BadRequest(new
					{
						Success = false,
						ErrorMessage = "Kullanıcı bulunamadı.",
						UserGuidance = "Geçerli bir kullanıcı adı girin ve tekrar deneyin."
					});
				}
			}
			catch (Exception ex)
			{
				return StatusCode(500, new
				{
					Success = false,
					ErrorMessage = "Kullanıcı bilgileri güncellenirken bir hata oluştu. Lütfen tekrar deneyin.",
					Details = ex.Message,
					UserGuidance = "Güncelleme işlemi sırasında bir hata oluştu. Tekrar deneyin veya destek ekibine başvurun."
				});
			}
		}
	}
}
