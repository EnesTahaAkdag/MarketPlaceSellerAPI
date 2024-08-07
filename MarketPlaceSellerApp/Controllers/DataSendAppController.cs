using MarketPlaceSellerApp.ViewModel;
using MarketPlaceSellerApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace MarketPlaceSellerApp.Controllers
{
    public class DataSendAppController : Controller
    {
         

        public ActionResult MarketPlaceData(int? page, int? pageSize)
        {
            try
            {
                int totalCount = db.Seller_Information.Count();
                int currentPage = page ?? 1;
                pageSize = pageSize ?? 50;
                int totalPage = (int)Math.Ceiling((decimal)totalCount / pageSize.GetValueOrDefault());

                var data = (from c in db.Seller_Information
                            orderby c.ID
                            select new HbInformationAppDataViewModel
                            {
                                Id = c.ID,
                                StoreName = c.StoreName,
                                Telephone = c.Telephone,
                                Email = c.Email,
                                Address = c.Address,
                                SellerName = c.SellerName
                            })
                            .Skip((currentPage - 1) * pageSize.GetValueOrDefault())
                            .Take(pageSize.GetValueOrDefault())
                            .ToList();

                return Json(new ApiResponse
                {
                    Success = true,
                    ErrorMessage = null,
                    Data = data,
                    Page = currentPage,
                    PageSize = pageSize.GetValueOrDefault(),
                    TotalCount = totalCount,
                    TotalPage = totalPage
                });
            }
            catch (Exception ex)
            {
                return Json(new ApiResponse
                {
                    Success = false,
                    ErrorMessage = ex.Message,
                });
            }
        }
    }
}