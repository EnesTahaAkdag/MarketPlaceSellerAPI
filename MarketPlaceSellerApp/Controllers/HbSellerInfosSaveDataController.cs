using MarketPlaceSellerApp.Models;
using MarketPlaceSellerApp.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace MarketPlaceSellerApp.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class HbSellerInfosSaveDataController : Controller
	{
		private readonly HepsiburadaSellerInformationContext _context;

		public HbSellerInfosSaveDataController(HepsiburadaSellerInformationContext context)
		{
			_context = context;
		}
		[HttpGet("GetRandomUrl")]
		public ActionResult GetRandomUrl()
		{
			var randomUrl = _context.SellerInformations
				.Where(r => r.Category == null || r.Category == "-" || r.Email == null || r.Email == "-")
				.OrderBy(r => Guid.NewGuid())
				.Select(r => r.Link)
				.FirstOrDefault();

			if (!string.IsNullOrEmpty(randomUrl))
			{
				return Json(new { success = true, url = randomUrl });
			}
			else
			{
				return Json(new { success = false, message = "Rastgele URL Bulunamadı." });
			}
		}
		[HttpPost("UpdateCategory")]
		public ActionResult UpdateCategory(SellerInformationViewModel model)
		{
			var dataControl = _context.SellerInformations.FirstOrDefault(m => m.StoreName == model.StoreName);
			if (dataControl == null)
			{
				return Json(new { success = false, message = "Böyle bir mağza Yok." });
			}
			else
			{
				dataControl.Category = model.Category;
				_context.SaveChanges();
				return Json(new { success = true, message = "Kategori başarıyla güncellendi", UpdateCategory = dataControl.Category });
			}
		}
		[HttpPost("UpdateData")]
		public ActionResult UpdateData(SellerInformationViewModel model)
		{
			var dataCheck = _context.SellerInformations.FirstOrDefault(m => m.StoreName == model.StoreName);
			if (dataCheck == null)
			{ return Json(new { success = false, message = "Böyle bir mağaza yok." }); }
			else
			{
				dataCheck.Email = model.Email;
				dataCheck.StoreScore = model.StoreScore;
				dataCheck.NumberOfRatings = model.NumberOfRatings;
				dataCheck.NumberOfFollowers = model.NumberOfFollowers;
				dataCheck.AverageDeliveryTime = model.AverageDeliveryTime;
				dataCheck.ResponseTime = model.ResponseTime;
				dataCheck.RatingScore = model.RatingScore;
				dataCheck.NumberOfComments = model.NumberOfComments;
				dataCheck.NumberOfProducts = model.NumberOfProducts;
				dataCheck.Vkn = model.VKN;
				try
				{
					_context.SaveChanges();
				}
				catch (Exception)
				{
					//
				}
				return Json(new { success = true, message = "Mağza başarıyla kaydedildi" });
			}
		}
	}
}