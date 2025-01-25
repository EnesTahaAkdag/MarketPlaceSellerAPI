using MarketPlaceSellerApp.Models;
using MarketPlaceSellerApp.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MarketPlaceSellerApp.Controllers
{
	[Authorize]
	[Route("[controller]")]
	[ApiController]
	public class UserDataSendApiController : ControllerBase
	{
		private readonly HepsiburadaSellerInformationContext _context;

		public UserDataSendApiController(HepsiburadaSellerInformationContext context)
		{
			_context = context;
		}

		[HttpGet("DataSend")]
		public async Task<IActionResult> DataSend(string userName)
		{
			if (string.IsNullOrEmpty(userName))
			{
				return BadRequest(new
				{
					Success = false,
					ErrorMessage = "Kullanıcı adı zorunludur. Lütfen kullanıcı adı alanını doldurunuz.",
					UserGuidance = "Devam etmek için geçerli bir kullanıcı adı girin."
				});
			}

			try
			{
				var user = await _context.UserData
					.Where(u => u.UserName == userName)
					.AsNoTracking()
					.Select(u => new
					{
						u.FirstName,
						u.LastName,
						u.UserName,
						u.Email,
						ProfileImageBase64 = string.IsNullOrEmpty(u.ProfileImage)
							? null
							: $"https://d255-37-130-115-91.ngrok-free.app/profile_images/{u.ProfileImage}"
					})
					.FirstOrDefaultAsync();

				if (user == null)
				{
					return NotFound(new
					{
						Success = false,
						ErrorMessage = "Kullanıcı bulunamadı.",
						UserGuidance = "Lütfen kullanıcı adı doğruluğunu kontrol edin."
					});
				}

				return Ok(new { Success = true, Data = user, Message = "Kullanıcı bilgileri başarıyla alındı." });
			}
			catch (Exception ex)
			{
				return StatusCode(500, new
				{
					Success = false,
					ErrorMessage = "Sunucu ile iletişimde bir sorun oluştu. Lütfen tekrar deneyin.",
					Details = ex.Message,
					UserGuidance = "Tekrar deneyiniz veya destek ekibiyle iletişime geçin."
				});
			}
		}
	}
}
