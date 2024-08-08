using MarketPlaceSellerApp.Models;
using MarketPlaceSellerApp.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
namespace MarketPlaceSellerApp.Controllers
{
	public class HbStNameAndStLinkSaveController : Controller
    {
		private readonly HepsiburadaSellerInformationContext _context;

		public HbStNameAndStLinkSaveController(HepsiburadaSellerInformationContext context)
		{
			_context = context;
		}
        public ActionResult Index()
        {
            var datalist = _context.SellerInformations.ToList();
            return View(datalist);
        }
        public ActionResult StNameAndStLink(SellerInformationViewModel model)
        {
            var datacontrol = _context.SellerInformations.FirstOrDefault(m => m.StoreName == model.StoreName);
            if (datacontrol != null)
            {
                return Json(new { success = false, message = "Mağza Zaten Kayıtlı" });
            }
            else
            {
                var newStore = new SellerInformation
                {
                    Link = model.Link,
                    StoreName = model.StoreName,
                };
				_context.SellerInformations.Add(newStore);
				_context.SaveChanges();
                return Json(new { success = true, message = "Yeni mağza başarıyla eklendi" });
            }
        }
    }
}