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
			try
			{
				var user = await _context.UserData.AsNoTracking().FirstOrDefaultAsync(m => m.UserName == model.UserName);

				if (user != null)
				{
					string hashPassword = HashingAndVerifyPassword.HashingPassword.HashPassword(model.Password);
					user.Password = hashPassword;

					_context.UserData.Update(user);
					await _context.SaveChangesAsync();
					return Ok(new { Success = true, Message = "Şifre Başarıyla Güncellendi" });
				}
				else
				{
					return BadRequest(new { Success = false, ErrorMessage = "Kullanıcı Bulunamadı" });
				}
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { Success = false, ErrorMessage = ex.Message });
			}
		}

		[HttpPost("EditUserData")]
		public async Task<IActionResult> UpdateUserData([FromBody] UpdateUserData model)
		{
			try
			{
				var user = await _context.UserData.FirstOrDefaultAsync(m => m.UserName == model.UserName);

				if (user != null)
				{
					user.FirstName = model.FirstName;
					user.LastName = model.LastName;
					user.Email = model.Email;
					user.Age = model.Age;
					if (!string.IsNullOrWhiteSpace(model.ProfileImageBase64) && !model.ProfileImageBase64.StartsWith("http",StringComparison.OrdinalIgnoreCase))
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
								: $"https://be65-37-130-115-91.ngrok-free.app/profile_images/{u.ProfileImage}"
						})
						.FirstOrDefaultAsync();

					return Ok(new ProfileUpdateApiResponse
					{
						Success = true,
						ErrorMessage = "Kullanıcı Bilgileri Başarıyla Güncellendi",
						Data = data
					});
				}
				else
				{
					return BadRequest(new { Success = false, ErrorMessage = "Kullanıcı Bulunamadı" });
				}
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { Success = false, ErrorMessage = ex.Message });
			}
		}
	}
}
