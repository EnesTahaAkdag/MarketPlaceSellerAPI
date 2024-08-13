using MarketPlaceSellerApp.Models;
using MarketPlaceSellerApp.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MarketPlaceSellerApp.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class RegisterAPIController : Controller
	{
		private readonly HepsiburadaSellerInformationContext _context;

		public RegisterAPIController(HepsiburadaSellerInformationContext context)
		{
			_context = context;
		}

		[HttpPost("RegisterUser")]
		public async Task<IActionResult> RegisterUser([FromBody] User model)
		{
			try
			{
				var dataControl = await _context.UserData.FirstOrDefaultAsync(m => m.UserName == model.UserName);
				if (dataControl == null)
				{
					var user = new UserDatum
					{
						FirstName = model.FirstName,
						LastName = model.LastName,
						UserName = model.UserName,
						Email = model.Email,
						Age = model.Age,
						Password = model.Password
					};

					_context.UserData.Add(user);
					await _context.SaveChangesAsync();

					return Json(new { Success = true , Message="Kullanıcı Başarıyla Kaydedildi"});
				}
				else
				{
					return BadRequest(new { Success = false, ErrorMessage = "Kullanıcı adı zaten mevcut." });
				}
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { Success = false, ErrorMessage = ex.Message });
			}
		}

	}
}
