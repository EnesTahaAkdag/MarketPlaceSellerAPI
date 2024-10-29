using MarketPlaceSellerApp.Models;
using MarketPlaceSellerApp.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MarketPlaceSellerApp.Controllers
{
	[Authorize]
	[Route("[controller]")]
	[ApiController]
	public class ApplicationContentApiController : ControllerBase
	{
		private readonly HepsiburadaSellerInformationContext _context;

		public ApplicationContentApiController(HepsiburadaSellerInformationContext context)
		{
			_context = context;
		}
		[HttpGet("MarketPlaceData")]
		public async Task<IActionResult> MarketPlaceData(int? page, int? pageSize)
		{
			try
			{
				int totalCount = _context.SellerInformations.Count();
				int currentPage = page ?? 1;
				pageSize = pageSize ?? 50;
				int totalPage = (int)Math.Ceiling((decimal)totalCount / pageSize.GetValueOrDefault());

				var data = await (from c in _context.SellerInformations
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
				  .AsNoTracking()
				  .ToListAsync();


				return Ok(new ApiResponse
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
				return BadRequest(new ApiResponse
				{
					Success = false,
					ErrorMessage = ex.Message,
				});
			}
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
								  select new SellerRatingChartViewModel
								  {
									  StoreName = c.StoreName,
									  RatingScore = c.RatingScore,
								  })
								  .Take(count)
								  .AsNoTracking()
								  .ToListAsync();

				var response = new ApiResponses
				{
					Success = true,
					ErrorMessage = null,
					Data = data,
					Count = count,
					TotalCount = totalCount
				};
				return Ok(response);
			}
			catch (Exception ex)
			{
				var response = new ApiResponses
				{
					Success = false,
					ErrorMessage = ex.Message
				};

				return BadRequest(response);
			}
		}
	}
}
