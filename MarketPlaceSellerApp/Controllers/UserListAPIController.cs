using MarketPlaceSellerApp.Models;
using MarketPlaceSellerApp.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;


namespace MarketPlaceSellerApp.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class UserListAPIController : Controller
	{
		private readonly HepsiburadaSellerInformationContext _context;

		public UserListAPIController(HepsiburadaSellerInformationContext context)
		{
			_context = context;
		}

		[HttpGet("UserList")]
		public async Task<ActionResult> UserList()
		{
			try
			{
				int count = _context.UserData.Count();
				int totalCount = await _context.UserData.CountAsync();
				var data = await (from c in _context.UserData
								  orderby c.Id
								  select new UserListViewModel
								  {
									  Id = c.Id,
									  Email = c.Email,
									  FirstName = c.FirstName,
									  LastName = c.LastName,
									  UserName = c.UserName,
									  Age = c.Age,
								  })
								  .Take(count)
								  .ToListAsync();
				var response = new APIResponse
				{
					Success = true,
					ErrorMessage = null,
					Data = data,
					TotalCount = totalCount
				};
				var json = JsonConvert.SerializeObject(response, new JsonSerializerSettings() { DateFormatString = "yyyy-MM-ddThh:mm:ssZ" });

				return Content(json,"application/json");
			}
			catch (Exception ex)
			{
				var response = new APIResponse
				{
					Success = false,
					ErrorMessage = ex.Message,
				};
				return Json (response );
			}
		}
	}
}