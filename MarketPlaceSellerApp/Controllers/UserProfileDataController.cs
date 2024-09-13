using MarketPlaceSellerApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MarketPlaceSellerApp.Controllers
{
	[Authorize]
	[ApiController]
	[Route("[controller]")]
	public class UserProfileDataController : Controller
	{
		private readonly HepsiburadaSellerInformationContext _context;

		public UserProfileDataController(HepsiburadaSellerInformationContext context)
		{
			_context = context;
		}

		[HttpGet("DataSend")]
		public async Task<IActionResult> DataSend(string userName)
		{
			if (string.IsNullOrEmpty(userName))
			{
				return BadRequest(new { Success = false, ErrorMessage = "Kullanıcı Adı Boş Olamaz" });
			}

			var user = await _context.UserData
				.Where(u => u.UserName == userName)
				.Select(u => new
				{
					u.FirstName,
					u.LastName,
					u.UserName,
					u.Email,
					u.Age,
					u.ProfileImage,
				})
				.FirstOrDefaultAsync();

			if (user == null)
			{
				return NotFound(new 
				{ 
					Success = false
					, ErrorMessage = "Kullanıcı Bulunamadı" 
				});
			}
			else
			{
			return Ok(new
			{
				Success = true,
				Data = user
			});
			}
		}
	}
}
