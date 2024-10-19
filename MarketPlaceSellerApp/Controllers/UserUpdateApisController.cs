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


		public UserUpdateApiController(HepsiburadaSellerInformationContext context,GuidOperation guidOperation)
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

		[HttpPost("UpdateUserProfileImage")]
		public async Task<IActionResult> UpdateUserProfilePhoto([FromBody] ProfilePhotoModel model)
		{
			if (model == null || string.IsNullOrEmpty(model.UserName) || string.IsNullOrEmpty(model.NewProfileImageBase64))
			{
				return BadRequest(new
				{
					Success = false,
					ErrorMessage = "Geçerli kullanıcı adı ve resim verisi sağlanmalıdır."
				});
			}

			try
			{
				var user = await _context.UserData.FirstOrDefaultAsync(m => m.UserName == model.UserName);

				if (user == null)
				{
					return NotFound(new
					{
						Success = false,
						ErrorMessage = "Kullanıcı bulunamadı."
					});
				}

				var profileImagePath = await _guidOperation.SaveProfileImageAsync(model.NewProfileImageBase64);

				if (string.IsNullOrEmpty(profileImagePath))
				{
					return BadRequest(new
					{
						Success = false,
						ErrorMessage = "Resim kaydedilirken bir hata oluştu."
					});
				}
				user.ProfileImage = profileImagePath;

				await _context.SaveChangesAsync();

				return Ok(new
				{
					Success = true,
					Message = "Profil resmi başarıyla güncellendi."
				});
			}
			catch (FormatException)
			{
				return BadRequest(new
				{
					Success = false,
					ErrorMessage = "Geçersiz resim formatı."
				});
			}
			catch (Exception ex)
			{
				return StatusCode(500, new
				{
					Success = false,
					ErrorMessage = $"Bir hata oluştu: {ex.Message}"
				});
			}
		}

		[HttpPost("EditUserData")]
		public async Task<IActionResult> UpdateUserData([FromBody] UpdateUser model)
		{
			try
			{
				var user = await _context.UserData.AsNoTracking().FirstOrDefaultAsync(m => m.UserName == model.UserName);

				if (user != null)
				{
					user.FirstName = model.FirstName;
					user.LastName = model.LastName;
					user.Email = model.Email;
					user.Age = model.Age;

					_context.UserData.Update(user);
					await _context.SaveChangesAsync();

					return Ok(new { Success = true, Message = "Kullanıcı Bilgileri Başarıyla Güncellendi" });
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
