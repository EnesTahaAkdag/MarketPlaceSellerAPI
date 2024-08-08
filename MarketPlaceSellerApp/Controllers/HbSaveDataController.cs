using MarketPlaceSellerApp.Models;
using MarketPlaceSellerApp.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace MarketPlaceSellerApp.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class HbSaveDataController : Controller
    {
		private readonly HepsiburadaSellerInformationContext _context;

		public HbSaveDataController(HepsiburadaSellerInformationContext context)
		{
			_context = context;
		}
		[HttpGet("GetRandomUrl")]
        public JsonResult GetRandomUrl()
        {
            var randomUrl = _context.SellerInformations
				 .Where(r => r.SellerName == null || r.SellerName == "-")
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
        [HttpPost("UpData")]
        public ActionResult UpData(SellerInformationViewModel model)
        {
            var dataControl = _context.SellerInformations.FirstOrDefault(m => m.StoreName == model.StoreName);
            if (dataControl == null)
            {
                return Json(new { success = false, message = "Böyle bir mağaza yok." });
            }
            else
            {
                dataControl.SellerName = model.SellerName;
                dataControl.Telephone = model.Telephone;
                dataControl.Address = model.Address;
                dataControl.Fax = model.Fax;
                dataControl.Mersis = model.Mersis;
                try
                {
					_context.SaveChanges();

                }
                catch (Exception)
                {
                    //
                }
                return Json(new { success = true, message = "Veri başarıyla güncellendi." });
            }
        }
    }
}