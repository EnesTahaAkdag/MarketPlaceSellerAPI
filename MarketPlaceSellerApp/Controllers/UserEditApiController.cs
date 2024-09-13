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
	public class UserEditApiController : Controller
	{
		private readonly HepsiburadaSellerInformationContext _context;

		public UserEditApiController(HepsiburadaSellerInformationContext context)
		{
			_context = context;
		}

		[HttpPost("EditUserData")]
		public async Task<IActionResult> UpdateUserData([FromBody] UpdateUser model)
		{
			try
			{
				var user = await _context.UserData.FirstOrDefaultAsync(m => m.UserName == model.UserName);

				if (user != null)
				{
					// Sadece değişen alanları güncelleyin
					user.FirstName = model.FirstName;
					user.LastName = model.LastName;
					user.Email = model.Email;
					user.Age = model.Age;

					_context.UserData.Update(user);
					await _context.SaveChangesAsync();

					return Json(new { Success = true, Message = "Kullanıcı Bilgileri Başarıyla Güncellendi" });
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