namespace MarketPlaceSellerApp.ViewModel
{
    public class SellerInformationModel
    {
		public string Email { get; set; }

		public string StoreName { get; set; }
        public decimal? StoreScore { get; set; }
        public int? NumberOfRatings { get; set; }
        public int? NumberOfFollowers { get; set; }
        public string AverageDeliveryTime { get; set; }
        public string ResponseTime { get; set; }
        public decimal? RatingScore { get; set; }
        public int? NumberOfComments { get; set; }
        public string NumberOfProducts { get; set; }
        public string VKN { get; set; }
    }
	public class SellerInfoCategoriModel
	{
		public string StoreName { get; set; }
		public string Category { get; set; }
	}

	public class SellerInfoLinkAndSTName
	{
		public string StoreName { get; set; }
		public string Link { get; set; }
	}

	public class SellerInfoPIFModel
    {
		public string SellerName { get; set; }
		public string Mersis { get; set; }
		public string Fax { get; set; }
		public string Address { get; set; }
		public string StoreName { get; set; }
		public string Telephone { get; set; }
	}

	public class StoreDetailsViewModel
    {
        public long Id { get; set; }
		public string Link { get; set; }
		public string StoreName { get; set; }
		public string Telephone { get; set; }
		public string Email { get; set; }
		public string Address { get; set; }
		public string SellerName { get; set; }

	}

	public class StoreDetailsApiResponse
    {
		public bool Success { get; set; }
		public string ErrorMessage { get; set; }
		public StoreDetailsViewModel Data { get; set; }
	}
    
	public class StoreLinkDeleteModel
	{
		public string Link { get; set; }
	}
}