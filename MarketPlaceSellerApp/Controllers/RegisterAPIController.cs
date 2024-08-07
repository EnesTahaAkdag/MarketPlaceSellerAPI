using MarketPlaceSellerApp.ViewModel;
using Microsoft.AspNetCore.Mvc;

namespace MarketPlaceSellerApp.Controllers
{
	public class RegisterAPIController : Controller
	{
		private readonly HepsiburadaSellerInformationEntities db = new HepsiburadaSellerInformationEntities;



		[HttpPost]
		public JsonResult RegisterUser([FromBody]RegisterDataViewModel model)
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

				db.UserData.Add(user);
				db.SaveChanges();

				return Json(new { Success = true });
			}
			catch (Exception ex)
			{
				return Json(new { Success = false, ErrorMessage = ex.Message });
			}
		}
	}
}
