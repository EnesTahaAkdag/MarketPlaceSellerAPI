using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Hepsiburada_Seller_Information.ViewModel
{
	public class SellerRaitingChartViewModel
	{
		public string StoreName { get; set; }
		public decimal? RatingScore { get; set; }
	}
	public class ApiResponses
	{
		public bool Success { get; set; }
		public string ErrorMessage { get; set; }
		public object Data { get; set; }
		public int Count { get; set; }
		public int TotalCount { get; set; }
	}
}