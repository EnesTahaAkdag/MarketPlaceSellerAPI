using Hepsiburada_Seller_Information.Models;
using Hepsiburada_Seller_Information.ViewModel;
using System;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Dapper;

namespace Hepsiburada_Seller_Information.Controllers
{
	public class SendDataToChartController : Controller
	{
		private readonly HepsiburadaSellerInformationEntities db = new HepsiburadaSellerInformationEntities();

		[HttpGet]
		public async Task<ActionResult> ChartData()
		{
			try
			{
				int count = db.Seller_Information.Count();
				int totalCount = await db.Seller_Information.CountAsync();
				var data = await (from c in db.Seller_Information
								  orderby c.ID
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
				var jsonResult = Json(response, JsonRequestBehavior.AllowGet);
				jsonResult.MaxJsonLength = int.MaxValue;
				return jsonResult;
			}
			catch (Exception ex)
			{
				var response = new ApiResponse
				{
					Success = false,
					ErrorMessage = ex.Message
				};

				return Json(response, JsonRequestBehavior.AllowGet);
			}
		}
	}
}
