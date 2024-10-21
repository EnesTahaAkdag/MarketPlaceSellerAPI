namespace MarketPlaceSellerApp.FileNameGuid
{
	public class GuidOperation
	{
		private readonly IWebHostEnvironment _webHostEnvironment;

		public GuidOperation(IWebHostEnvironment webHostEnvironment)
		{
			_webHostEnvironment = webHostEnvironment;
		}

		public async Task<string> SaveProfileImageAsync(string profileImageBase64)
		{
			try
			{
				byte[] imageBytes = Convert.FromBase64String(profileImageBase64);
				var fileName = $"{Guid.NewGuid():N}.png";
				var filePath = Path.Combine(_webHostEnvironment.WebRootPath ?? _webHostEnvironment.ContentRootPath, "profile_images");
				
				if (!Directory.Exists(filePath))
					Directory.CreateDirectory(filePath);

				var fullFilePath = Path.Combine(filePath, fileName);
				await File.WriteAllBytesAsync(fullFilePath, imageBytes);
				return fileName;
			}
			catch (Exception ex)
			{
				throw new InvalidOperationException("Profil resmi kaydedilemedi.", ex);
			}
		}
	}

}
