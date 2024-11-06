
namespace MarketPlaceSellerApp.ViewModel
{
	public class HbInformationAppDataViewModel
	{
		public long Id { get; set; }
		public string StoreName { get; set; }
		public string Telephone { get; set; }
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