using MarketPlaceSellerApp.Models;
using MarketPlaceSellerApp.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

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
		public JsonResult RegisterUser([FromBody] RegisterDataViewModel model)
		{
			if (!ModelState.IsValid)
			{
				var errors = ModelState.Values.SelectMany(v => v.Errors)
											  .Select(e => e.ErrorMessage)
											  .ToList();
				return Json(new { Success = false, ErrorMessage = "Geçersiz veri girdiniz.", Errors = errors });
			}

			try
			{
				var user = new RegisterDataViewModel
				{
					FirstName = model.FirstName,
					LastName = model.LastName,
					UserName = model.UserName,
					Email = model.Email,
					Age = model.Age,
					Password = model.Password
				};

				//_context.UserData.Add(user);
				_context.SaveChanges();

				return Json(new { Success = true });
			}
			catch (Exception ex)
			{
				return Json(new { Success = false, ErrorMessage = ex.Message });
			}
		}
	}
}
