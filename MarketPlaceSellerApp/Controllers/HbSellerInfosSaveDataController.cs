using Hepsiburada_Seller_Information.Models;
using Hepsiburada_Seller_Information.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Hepsiburada_Seller_Information.Controllers
{
	public class HbSellerInfosSaveDataController : Controller
	{
		private HepsiburadaSellerInformationEntities db = new HepsiburadaSellerInformationEntities();
		[HttpGet]
		public ActionResult GetRandomUrl()
		{
			var randomUrl = db.Seller_Information
				.Where(r => r.Category == null || r.Category == "-" || r.Email == null || r.Email == "-")
				.OrderBy(r => Guid.NewGuid())
				.Select(r => r.Link)
				.FirstOrDefault();

			if (!string.IsNullOrEmpty(randomUrl))
			{
				return Json(new { success = true, url = randomUrl }, JsonRequestBehavior.AllowGet);
			}
			else
			{
				return Json(new { success = false, message = "Rastgele URL Bulunamadı." }, JsonRequestBehavior.AllowGet);
			}
		}
		[HttpPost]
		public ActionResult UpdateCategory(SellerInformationViewModel model)
		{
			var dataControl = db.Seller_Information.FirstOrDefault(m => m.StoreName == model.StoreName);
			if (dataControl == null)
			{
				return Json(new { success = false, message = "Böyle bir mağza Yok." });
			}
			else
			{
				dataControl.Category = model.Category;
				db.SaveChanges();
				return Json(new { success = true, message = "Kategori başarıyla güncellendi", UpdateCategory = dataControl.Category });
			}
		}
		[HttpPost]
		public ActionResult UpdateData(SellerInformationViewModel model)
		{
			var dataCheck = db.Seller_Information.FirstOrDefault(m => m.StoreName == model.StoreName);
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
				dataCheck.VKN = model.VKN;
				try
				{
					db.SaveChanges();
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