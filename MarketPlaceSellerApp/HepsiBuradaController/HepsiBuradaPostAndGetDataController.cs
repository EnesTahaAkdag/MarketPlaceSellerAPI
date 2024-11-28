using MarketPlaceSellerApp.Models;
using MarketPlaceSellerApp.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Dapper;
using Microsoft.Data.SqlClient;

namespace MarketPlaceSellerApp.HepsiBuradaController
{
	[Route("[controller]")]
	[ApiController]
	public class HepsiBuradaPostAndGetDataApiController : Controller
	{
		private readonly HepsiburadaSellerInformationContext _context;
		private readonly ILogger<HepsiBuradaPostAndGetDataApiController> _logger;
		private readonly string _connectionString;

		public HepsiBuradaPostAndGetDataApiController(HepsiburadaSellerInformationContext context, ILogger<HepsiBuradaPostAndGetDataApiController> logger)
		{
			_context = context;
			_logger = logger;
			_connectionString = "Data Source=PRASOFT\\SQLEXPRESS;Initial Catalog=Hepsiburada-Seller-Information;Persist Security Info=True;Trusted_Connection=True;TrustServerCertificate=Yes;";
		}

		[HttpDelete("DeleteUrlDb")]
		public async Task<IActionResult> DeleteUrlDb([FromBody] StoreLinkDeleteModel model)
		{
			if (model == null || string.IsNullOrWhiteSpace(model.Link))
				return BadRequest(new { success = false, message = "Geçersiz URL." });

			try
			{

				using (var connection = new SqlConnection(_connectionString))
				{
					var sqlQuery = @"DELETE FROM [Hepsiburada-Seller-Information].[dbo].[Seller_Information]
                             WHERE [Link] = @Link";

					var rowsAffected = await connection.ExecuteAsync(sqlQuery, new { Link = model.Link });

					if (rowsAffected > 0)
						return Ok(new { success = true, message = "Mağaza başarıyla silindi." });

					return NotFound(new { success = false, message = "Mağaza bulunamadı." });
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Silme işlemi sırasında bir hata oluştu.");
				return StatusCode(500, new { success = false, message = "Sunucu hatası." });
			}
		}


		[HttpPost("GetRandomUrlDelete")]
		public async Task<JsonResult> GetRandomUrlDelete()
		{

			var sqlQuery = @"  
        SELECT TOP 1 [Link]
        FROM [Hepsiburada-Seller-Information].[dbo].[Seller_Information]
        WHERE Telephone IS NOT NULL
          AND StoreScore IS NOT NULL
          AND NumberOfProducts IS NOT NULL
          AND (
            TRY_CAST(NumberOfProducts AS int) >= 20
            OR NumberOfProducts IN ('1B', '2B', '3B', '4B', '5B', '6B', '7B', '8B', '9B', 
                                    '10B', '11B', '12B', '13B', '14B', '15B', '16B', '17B', '18B', '19B', '20B')
          )
          AND SellerName IS NOT NULL
          AND LEFT(Telephone, 1) = '0'
        ORDER BY NEWID()";

			try
			{
				using (var connection = new SqlConnection(_connectionString))
				{
					connection.Open();

					var randomUrl = await connection.QuerySingleOrDefaultAsync<string>(sqlQuery);

					if (string.IsNullOrEmpty(randomUrl))
					{
						return Json(new { success = false, message = "Rastgele URL bulunamadı." });
					}

					return Json(new
					{
						success = true,
						message = "Rastgele URL başarıyla getirildi.",
						url = randomUrl
					});
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Rastgele URL getirilirken bir hata oluştu.");
				return Json(new { success = false, message = "Bir hata oluştu.", error = ex.Message });
			}
		}

		[HttpGet("Index")]
		public async Task<IActionResult> Index()
		{
			var datalist = await _context.SellerInformations.AsNoTracking().ToListAsync();
			return View(datalist);
		}

		[HttpPost("GetRandomUrl")]
		public async Task<JsonResult> GetRandomUrl()
		{
			const string sqlQuery = @"
SELECT TOP 1 [Link]
FROM [Hepsiburada-Seller-Information].[dbo].[Seller-Information]
WHERE
([SellerName]IS NULL Or [SellerName]='')
AND [Category] IS NOT NULL
AND TRY_CAST([NumberOfProducts]AS int)>20
ORDER BY NEWID();";

			try
			{
				using (var connection = new SqlConnection(_connectionString))
				{
					connection.Open();

					var randomUrl = await connection.QuerySingleOrDefaultAsync<string>(sqlQuery);

					return Json(new
					{
						success = !string.IsNullOrEmpty(randomUrl),
						url = randomUrl ?? "Rastgele URL Bulunamadı"
					});
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Rastgele Url Getirilirken bir hata oluştu.");

				return Json(new
				{
					success = false,
					message = "Bir hata oluştu.",
					error = ex.Message
				});
			}
		}



		[HttpPost("UpData")]
		public async Task<JsonResult> UpData([FromBody] SellerInfoPIFModel model)
		{
			if (!ModelState.IsValid) return Json(new { success = false, message = "Geçersiz veri." });

			var dataControl = await _context.SellerInformations.FirstOrDefaultAsync(m => m.StoreName == model.StoreName);
			if (dataControl == null) return Json(new { success = false, message = "Böyle bir mağaza yok." });

			if (!string.IsNullOrWhiteSpace(model.SellerName) && model.SellerName.StartsWith("Ünvanı:"))
				dataControl.SellerName = model.SellerName.Replace("Ünvanı:", "").Trim();
			else if (string.IsNullOrWhiteSpace(model.SellerName))
				dataControl.SellerName = null;

			if (!string.IsNullOrWhiteSpace(model.Address) && model.Address.StartsWith("Adresi:"))
				dataControl.Address = model.Address.Replace("Adresi:", "").Trim();
			else if (string.IsNullOrWhiteSpace(model.Address))
				dataControl.Address = null;

			if (!string.IsNullOrWhiteSpace(model.Fax) && model.Fax.StartsWith("Fax:"))
			{
				var faxValue = model.Fax.Replace("Fax:", "").Trim();
				dataControl.Fax = string.IsNullOrWhiteSpace(faxValue) ? null : faxValue;
			}
			else if (string.IsNullOrWhiteSpace(model.Fax))
			{
				dataControl.Fax = null;
			}

			if (!string.IsNullOrWhiteSpace(model.Mersis) && model.Mersis.StartsWith("Mersis Numarası:"))
				dataControl.Mersis = model.Mersis.Replace("Mersis Numarası:", "").Trim();
			else if (string.IsNullOrWhiteSpace(model.Mersis))
				dataControl.Mersis = null;

			dataControl.Telephone = string.IsNullOrWhiteSpace(model.Telephone) ? null : model.Telephone;

			try
			{
				await _context.SaveChangesAsync();
				return Json(new { success = true, message = "Veri başarıyla güncellendi." });
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Veri güncelleme hatası.");
				return Json(new { success = false, message = "Veri güncelleme başarısız." });
			}
		}



		[HttpPost("GetRandomUrls")]
		public async Task<JsonResult> GetRandomUrls()
		{
			var randomUrl = await _context.SellerInformations
				.Where(r => string.IsNullOrEmpty(r.Category) || r.Category == "-" || string.IsNullOrEmpty(r.Email) || r.Email == "-")
				.OrderBy(r => Guid.NewGuid())
				.Select(r => r.Link)
				.FirstOrDefaultAsync();

			return Json(new { success = !string.IsNullOrEmpty(randomUrl), url = randomUrl ?? "Rastgele URL Bulunamadı." });
		}

		[HttpPost("UpdateCategory")]
		public async Task<JsonResult> UpdateCategory([FromBody] SellerInfoCategoriModel model)
		{
			if (!ModelState.IsValid) return Json(new { success = false, message = "Geçersiz veri." });

			var dataControl = await _context.SellerInformations.FirstOrDefaultAsync(m => m.StoreName == model.StoreName);
			if (dataControl == null) return Json(new { success = false, message = "Böyle bir mağaza yok." });

			dataControl.Category = model.Category;
			await _context.SaveChangesAsync();
			return Json(new { success = true, message = "Kategori başarıyla güncellendi.", updateCategory = dataControl.Category });
		}

		[HttpPost("UpdateData")]
		public async Task<JsonResult> UpdateData([FromBody] SellerInformationModel model)
		{
			if (!ModelState.IsValid) return Json(new { success = false, message = "Geçersiz veri." });

			var dataCheck = await _context.SellerInformations.FirstOrDefaultAsync(m => m.StoreName == model.StoreName);
			if (dataCheck == null) return Json(new { success = false, message = "Böyle bir mağaza yok." });

			dataCheck.Email = model.Email;
			dataCheck.StoreScore = model.StoreScore;
			dataCheck.NumberOfRatings = model.NumberOfRatings;
			dataCheck.NumberOfFollowers = model.NumberOfFollowers;
			dataCheck.AverageDeliveryTime = model.AverageDeliveryTime;
			dataCheck.ResponseTime = model.ResponseTime;
			dataCheck.RatingScore = model.RatingScore;
			dataCheck.NumberOfComments = model.NumberOfComments;
			dataCheck.NumberOfProducts = model.NumberOfProducts;
			dataCheck.Vkn = model.VKN;

			try
			{
				await _context.SaveChangesAsync();
				return Json(new { success = true, message = "Mağaza başarıyla güncellendi" });
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Veri güncelleme hatası.");
				return Json(new { success = false, message = "Mağaza güncelleme başarısız." });
			}
		}

		[HttpPost("StNameAndStLink")]
		public async Task<JsonResult> StNameAndStLink([FromBody] SellerInfoLinkAndSTName model)
		{
			if (!ModelState.IsValid) return Json(new { success = false, message = "Geçersiz veri." });

			var dataCheck = await _context.SellerInformations.AnyAsync(m => m.StoreName == model.StoreName);

			if (dataCheck) return Json(new { success = false, message = "Mağaza zaten kayıtlı" });

			var dataControl = await _context.SellerInformations.FirstOrDefaultAsync(m => m.StoreName == model.StoreName);

			dataControl.Link = model.Link;
			dataControl.StoreName = model.StoreName;

			await _context.SaveChangesAsync();

			return Json(new { success = true, message = "Yeni mağaza başarıyla eklendi" });
		}
	}
}
