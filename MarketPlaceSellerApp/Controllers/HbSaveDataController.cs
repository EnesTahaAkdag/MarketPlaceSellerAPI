using MarketPlaceSellerApp.ViewModel;
using System;
using System.Linq;
using System.Web.Mvc;

namespace MarketPlaceSellerApp.Controllers
{
    public class HbSaveDataController : Controller
    {
        private HepsiburadaSellerInformationEntities db = new HepsiburadaSellerInformationEntities();
        [HttpGet]
        public JsonResult GetRandomUrl()
        {
            var randomUrl = db.Seller_Information
				 .Where(r => r.SellerName == null || r.SellerName == "-")
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
        public ActionResult UpData(SellerInformationViewModel model)
        {
            var dataControl = db.Seller_Information.FirstOrDefault(m => m.StoreName == model.StoreName);
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
                    db.SaveChanges();

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