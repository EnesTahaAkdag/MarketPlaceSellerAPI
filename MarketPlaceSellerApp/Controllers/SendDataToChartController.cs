using MarketPlaceSellerApp.Models;
using MarketPlaceSellerApp.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
namespace MarketPlaceSellerApp.Controllers
{
	[ApiController]
		[Route("[controller]")]
	public class SendDataToChartController : Controller
	{	
		private readonly HepsiburadaSellerInformationContext _context;

		public SendDataToChartController(HepsiburadaSellerInformationContext context)
		{
			_context = context;
		}
		[HttpGet("ChartData")]
		public async Task<IActionResult> ChartData()
		{
			try
			{
				int count = _context.SellerInformations.Count();
				int totalCount = await _context.SellerInformations.CountAsync();
				var data = await (from c in _context.SellerInformations
								  orderby c.Id
								  select new SellerRaitingChartViewModel
								  {
									  StoreName = c.StoreName,
									  RatingScore = c.RatingScore,
								  })
								  .Take(count)
								  .ToListAsync();

				var response = new ApiResponses
				{
					Success = true,
					ErrorMessage = null,
					Data = data,
					Count = count,
					TotalCount = totalCount
				};
				return Json(response);
			}
			catch (Exception ex)
			{
				var response = new ApiResponse
				{
					Success = false,
					ErrorMessage = ex.Message
				};

				return Json(response);
			}
		}
	}
}
