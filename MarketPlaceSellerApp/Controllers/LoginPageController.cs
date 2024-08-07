using Hepsiburada_Seller_Information.ViewModel;
namespace Hepsiburada_Seller_Information.Controllers
{
    public class LoginPageController : Controller
    {
        private HepsiburadaSellerInformationEntities db =new HepsiburadaSellerInformationEntities();

        public ActionResult Index(LoginDataViewModel model)
        {

            return View();
        }
    }
}