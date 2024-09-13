using MarketPlaceSellerApp.Models;
using MarketPlaceSellerApp.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
namespace MarketPlaceSellerApp.Controllers
{
	[Authorize]
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
								  select new UserList
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
				var response = new UserApiResponse
				{
					Success = true,
					ErrorMessage = null,
					Data = data,
					TotalCount = totalCount
				};
				var json = JsonConvert.SerializeObject(response, new JsonSerializerSettings() { DateFormatString = "dd-MM-yyyy" });

				return Content(json, "application/json");
			}
			catch (Exception ex)
			{
				var response = new UserApiResponse
				{
					Success = false,
					ErrorMessage = ex.Message,
				};
				return Json(response);
			}
		}
	}
}