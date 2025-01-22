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

					var rowsAffected = await connection.ExecuteAsync(sqlQuery, new { model.Link });

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
'10B', '11B', '12B', '13B', '14B', '15B', '16B', '17B', '18B', '19B', '20B'))
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

		[HttpPost("GetRandomUrl")]
		public async Task<JsonResult> GetRandomUrl()
		{
			string sqlQuery = @"
SELECT TOP 1 [Link]
FROM [Hepsiburada-Seller-Information].[dbo].[Seller_Information]
WHERE
([SellerName] IS NULL OR [SellerName] = '')
AND [Category] IS NOT NULL   
AND TRY_CAST([NumberOfProducts] AS int) > 20
ORDER BY NEWID();";

			try
			{
				using (var connection = new SqlConnection(_connectionString))
				{
					await connection.OpenAsync();

					var randomUrl = await connection.QuerySingleOrDefaultAsync<string>(sqlQuery);

					if (string.IsNullOrEmpty(randomUrl))
					{
						return Json(new { success = false, message = "Rastgele URL Bulunamadı" });
					}

					return Json(new
					{
						success = !string.IsNullOrEmpty(randomUrl),
						url = randomUrl ?? "Uygun URL bulunamadı."
					});
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Rastgele URL getirilirken bir hata oluştu.");

				return Json(new
				{
					success = false,
					message = "Beklenmeyen bir hata oluştu.",
					error = ex.Message
				});
			}
		}

		[HttpPost("UpData")]
		public async Task<JsonResult> UpData([FromBody] SellerInfoPIFModel model)
		{
			if (!ModelState.IsValid)
				return Json(new { success = false, message = "Geçersiz veri." });

			// Veritabanında mağaza adı ile kontrol
			var dataControl = await _context.SellerInformations.FirstOrDefaultAsync(m => m.StoreName == model.StoreName);
			if (dataControl == null)
				return Json(new { success = false, message = "Böyle bir mağaza yok." });

			// Aynı "SellerName" veritabanında zaten mevcutsa işlem yapılmaz
			var existingSeller = await _context.SellerInformations
				.FirstOrDefaultAsync(m => m.SellerName == model.SellerName);
			if (existingSeller != null)
				return Json(new { success = false, message = "Bu 'SellerName' başka bir mağaza tarafından kullanılıyor." });

			// Verileri düzenle ve kontrol et
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
			{
				var mersisValue = model.Mersis.Replace("Mersis Numarası:", "").Trim();
				dataControl.Mersis = string.IsNullOrEmpty(mersisValue) ? null : mersisValue;
			}
			else if (string.IsNullOrWhiteSpace(model.Mersis))
			{
				dataControl.Mersis = null;
			}

			dataControl.Telephone = string.IsNullOrWhiteSpace(model.Telephone) ? null : model.Telephone;

			try
			{
				// Veritabanında değişiklikleri kaydet
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
			string sqlQuery = @"
SELECT TOP 1 [Link] 
FROM [Hepsiburada-Seller-Information].[dbo].[Seller_Information]
WHERE ([Category] IS NULL AND [Email] IS NULL) ORDER BY NEWID();";

			try
			{
				using (var connection = new SqlConnection(_connectionString))
				{
					await connection.OpenAsync();

					var randomURL = await connection.QuerySingleOrDefaultAsync<string>(sqlQuery);

					if (string.IsNullOrEmpty(randomURL))
					{
						return Json(new
						{
							success = false,
							message = "Rastgele URL Bulunamadı"
						});
					}
					return Json(new
					{
						success = true,
						message = randomURL ?? "Uygun Url Bulunamadı"
					});
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Rastgele Url Getirilirken bir Hata Oluştu");

				return Json(new
				{
					success = false,
					message = "Beklenmeyen bir hata meydana geldi",
					error = ex.Message
				});
			}
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
