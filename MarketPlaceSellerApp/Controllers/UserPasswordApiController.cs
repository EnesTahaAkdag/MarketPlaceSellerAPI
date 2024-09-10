using MarketPlaceSellerApp.HashingPassword;
using MarketPlaceSellerApp.Models;
using MarketPlaceSellerApp.ViewModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MarketPlaceSellerApp.Controllers
{
	[Route("[controller]")]
	[ApiController]
	public class UserPasswordApiController : ControllerBase
	{
		private readonly HepsiburadaSellerInformationContext _context;

		public UserPasswordApiController(HepsiburadaSellerInformationContext context)
		{
			_context = context;
		}

		[HttpPost("UpdatePassword")]
		public async Task<IActionResult> UpdatePassword([FromBody] UpdatePassword model)
		{
			try
			{
				var user = await _context.UserData.FirstOrDefaultAsync(m => m.UserName == model.UserName);

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
				return StatusCode(500 , new {Success = false,ErrorMessage=ex.Message});
			}
		}
	}
}
