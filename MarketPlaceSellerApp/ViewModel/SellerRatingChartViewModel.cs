namespace MarketPlaceSellerApp.ViewModel
{
    public class SellerRatingChartViewModel
    {
        public decimal? RatingScore { get; set; }
        public int Count { get; set; }
        public bool IsNullValue { get; set; }
    }

    public class ApiResponses
    {
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
        public List<SellerRatingChartViewModel> Data { get; set; }
        public int Count { get; set; }
        public int TotalCount { get; set; }
        public int NullValueCount { get; set; }
    }
} 