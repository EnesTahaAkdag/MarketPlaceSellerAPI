using Hepsiburada_Seller_Information.Models;
using System;
using System.Linq;
using System.Web.Mvc;
using Hepsiburada_Seller_Information.ViewModel;
using System.Data.Entity;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Hepsiburada_Seller_Information.Controllers
{
	public class UserListAPIController : Controller
	{
		private HepsiburadaSellerInformationEntities db = new HepsiburadaSellerInformationEntities();
		[HttpGet]
		public async Task<ActionResult> UserList(LoginDataViewModel model)
		{
			try
			{
				int count = db.UserData.Count();
				int totalCount = await db.UserData.CountAsync();
				var data = await (from c in db.UserData
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
				return Json (response, JsonRequestBehavior.AllowGet);
			}
		}
	}
}