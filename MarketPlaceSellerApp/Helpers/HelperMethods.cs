using Newtonsoft.Json.Converters;

namespace MarketPlaceSellerApp.Helpers
{


	public class DateFormatConverter : IsoDateTimeConverter
	{
		public DateFormatConverter(string format)
		{
			DateTimeFormat = format;
		}
	}

}