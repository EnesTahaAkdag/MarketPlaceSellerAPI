using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Hepsiburada_Seller_Information.ViewModel
{
	public class HbInformationAppDataViewModel
	{
		public long Id { get; set; }
		public string StoreName { get; set; }
		public string Telephone { get; set; }
		public string Email { get; set; }
		public string Address { get; set; }
		public string SellerName { get; set; }
	}
	public class ApiResponse
	{
		public bool Success { get; set; }
		public string ErrorMessage { get; set; }
		public object Data { get; set; }
		public int Page { get; set; }
		public int PageSize { get; set; }
		public int TotalCount { get; set; }
		public int TotalPage { get; set; }
	}
}