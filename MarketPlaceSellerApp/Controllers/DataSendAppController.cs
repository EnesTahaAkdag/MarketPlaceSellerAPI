﻿using MarketPlaceSellerApp.Models;
using MarketPlaceSellerApp.ViewModel;
using Microsoft.AspNetCore.Mvc;

namespace MarketPlaceSellerApp.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class DataSendAppController : Controller
    {
		private readonly HepsiburadaSellerInformationContext _context;

		public DataSendAppController(HepsiburadaSellerInformationContext context)
		{
			_context = context;
		}
        [HttpGet("MarketPlaceData")]
		public ActionResult MarketPlaceData(int? page, int? pageSize)
        {
            try
            {
                int totalCount = _context.SellerInformations.Count();
                int currentPage = page ?? 1;
                pageSize = pageSize ?? 50;
                int totalPage = (int)Math.Ceiling((decimal)totalCount / pageSize.GetValueOrDefault());

                var data = (from c in _context.SellerInformations
                            orderby c.Id
                            select new HbInformationAppDataViewModel
                            {
                                Id = c.Id,
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