using MarketPlaceSellerApp.Models;
using MarketPlaceSellerApp.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace MarketPlaceSellerApp.Controllers
{
	//[Authorize]
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
				page = Math.Max(page ?? 1, 1);
				pageSize = Math.Clamp(pageSize ?? 50, 1, 100);

				var query = _context.SellerInformations
					.AsNoTracking()
					.OrderBy(c => c.Id);

				var totalCount = await query.CountAsync();
				var totalPage = (int)Math.Ceiling(totalCount / (double)pageSize.Value);

				var data = await query
					.Skip((page.Value - 1) * pageSize.Value)
					.Take(pageSize.Value)
					.Select(c => new HbInformationAppDataViewModel
					{
						Id = c.Id,
						StoreName = c.StoreName,
						Telephone = c.Telephone,
					})
					.ToListAsync();

				return Ok(new ApiResponse
				{
					Success = true,
					ErrorMessage = null,
					Data = data,
					Page = page.Value,
					PageSize = pageSize.Value,
					TotalCount = totalCount,
					TotalPage = totalPage
				});
			}
			catch (Exception ex)
			{
				return StatusCode(500, new ApiResponse
				{
					Success = false,
					ErrorMessage = $"Sunucu hatası: {ex.Message}"
				});
			}
		}

		[HttpGet("StoreDetails")]
		public async Task<IActionResult> StoreDetails(long id)
		{
			if (id <= 0)
			{
				return BadRequest(new StoreDetailsApiResponse
				{
					Success = false,
					ErrorMessage = "Geçersiz mağaza kimliği."
				});
			}

			try
			{
				var storeDetails = await _context.SellerInformations
					.AsNoTracking()
					.Where(c => c.Id == id)
					.Select(c => new StoreDetailsViewModel
					{
						Id = c.Id,
						StoreName = c.StoreName ?? string.Empty,
						SellerName = c.SellerName ?? string.Empty,
						Telephone = c.Telephone ?? string.Empty,
						Email = c.Email ?? string.Empty,
						Address = c.Address ?? string.Empty,
						Link = c.Link ?? string.Empty
					})
					.FirstOrDefaultAsync();

				if (storeDetails == null)
				{
					return NotFound(new StoreDetailsApiResponse
					{
						Success = false,
						ErrorMessage = $"Mağaza bulunamadı. ID: {id}"
					});
				}

				return Ok(new StoreDetailsApiResponse
				{
					Success = true,
					Data = storeDetails
				});
			}
			catch (Exception ex)
			{
				return StatusCode(500, new StoreDetailsApiResponse
				{
					Success = false,
					ErrorMessage = $"Sunucu hatası: {ex.Message}"
				});
			}
		}

		[HttpGet("ChartData")]
		public async Task<IActionResult> ChartData()
		{
			try
			{
				var allData = await _context.SellerInformations
					.Select(s => new { s.RatingScore })
					.ToListAsync();

				var ratings = allData
					.Where(s => s.RatingScore.HasValue)
					.Select(s => s.RatingScore.Value)
					.ToList();

				var nullCount = allData.Count(s => !s.RatingScore.HasValue);

				if (!ratings.Any() && nullCount == 0)
				{
					return Ok(new ApiResponses
					{
						Success = true,
						ErrorMessage = "Veri bulunmamaktadır.",
						Data = new List<SellerRatingChartViewModel>(),
						Count = 0,
						TotalCount = 0,
						NullValueCount = 0
					});
				}

				var ratingPoints = Enumerable.Range(1, 5)
					.Select(rating => new SellerRatingChartViewModel
					{
						RatingScore = rating,
						Count = ratings.Count(r => Math.Floor(r) == rating)
					})
					.Where(r => r.Count > 0)
					.OrderBy(r => r.RatingScore)
					.ToList();

				return Ok(new ApiResponses
				{
					Success = true,
					ErrorMessage = null,
					Data = ratingPoints,
					Count = ratingPoints.Count,
					TotalCount = allData.Count,
					NullValueCount = nullCount
				});
			}
			catch (Exception ex)
			{
				return StatusCode(500, new ApiResponses
				{
					Success = false,
					ErrorMessage = $"Sunucu hatası: {ex.Message}"
				});
			}
		}
	}
}
