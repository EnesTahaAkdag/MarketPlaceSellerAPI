using Hepsiburada_Seller_Information.Models;
using Hepsiburada_Seller_Information.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Management;
using System.Web.Mvc;
using System.Web.Services.Description;

namespace Hepsiburada_Seller_Information.Controllers
{
    public class HbStNameAndStLinkSaveController : Controller
    {
        private HepsiburadaSellerInformationEntities db = new HepsiburadaSellerInformationEntities();
        [HttpGet]
        public ActionResult Index()
        {
            var datalist = db.Seller_Information.ToList();
            return View(datalist);
        }
        public ActionResult StNameAndStLink(SellerInformationViewModel model)
        {
            var datacontrol = db.Seller_Information.FirstOrDefault(m => m.StoreName == model.StoreName);
            if (datacontrol != null)
            {
                return Json(new { success = false, message = "Mağza Zaten Kayıtlı" });
            }
            else
            {
                var newStore = new Seller_Information
                {
                    Link = model.Link,
                    StoreName = model.StoreName,
                };
                db.Seller_Information.Add(newStore);
                db.SaveChanges();
                return Json(new { success = true, message = "Yeni mağza başarıyla eklendi" });
            }
        }
    }
}