using MarketPlaceSellerApp.Models;
using MarketPlaceSellerApp.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
namespace MarketPlaceSellerApp.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class LoginPageController : Controller
	{
		private readonly HepsiburadaSellerInformationContext _context;

		public LoginPageController(HepsiburadaSellerInformationContext context)
		{
			_context = context;
		}
		[HttpGet("LoginUserData")]
		public ActionResult LoginUserData(LoginUserViewModel model)
		{

			return View();
		}
	}
}