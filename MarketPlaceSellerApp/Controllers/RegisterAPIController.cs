using MarketPlaceSellerApp.HashingPassword;
using MarketPlaceSellerApp.Models;
using MarketPlaceSellerApp.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MarketPlaceSellerApp.Controllers
{
	[AllowAnonymous]
	[ApiController]
	[Route("[controller]")]
	public class RegisterAPIController : Controller
	{
		private readonly HepsiburadaSellerInformationContext _context;
		private readonly IWebHostEnvironment _webHostEnvironment;

		public RegisterAPIController(HepsiburadaSellerInformationContext context, IWebHostEnvironment webHostEnvironment)
		{
			_context = context;
			_webHostEnvironment = webHostEnvironment;
		}

		[HttpPost("RegisterUser")]
		public async Task<IActionResult> RegisterUser([FromForm] User model, IFormFile profileImage)
		{
			// Validate the model state
			if (!ModelState.IsValid)
			{
				return BadRequest(new { Success = false, ErrorMessage = "Geçersiz model girdisi." });
			}

			try
			{
				// Check if the username or email is already in use
				var existingUser = await _context.UserData
					.AsNoTracking()
					.FirstOrDefaultAsync(m => m.UserName == model.UserName || m.Email == model.Email);

				if (existingUser != null)
				{
					if (existingUser.UserName == model.UserName)
					{
						return BadRequest(new { Success = false, ErrorMessage = "Kullanıcı adı zaten mevcut." });
					}
					if (existingUser.Email == model.Email)
					{
						return BadRequest(new { Success = false, ErrorMessage = "Email adresi zaten kayıtlı." });
					}
				}

				// Hash the password before saving to the database
				string hashPassword = HashingAndVerifyPassword.HashingPassword.HashPassword(model.Password);

				// Handle profile image upload
				string profileImagePath = null;

				if (profileImage != null && profileImage.Length > 0)
				{
					// Generate a unique filename using GUID
					var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(profileImage.FileName);

					// Get the path to save the image
					var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "profile_images");
					if (!Directory.Exists(uploadsFolder))
					{
						Directory.CreateDirectory(uploadsFolder);
					}

					// Create the full path
					profileImagePath = Path.Combine(uploadsFolder, uniqueFileName);

					// Copy the file to the target folder
					using (var fileStream = new FileStream(profileImagePath, FileMode.Create))
					{
						await profileImage.CopyToAsync(fileStream);
					}

					// Set the path to save in the database
					profileImagePath = Path.Combine("uploads", "profile_images", uniqueFileName);
				}

				// Create new UserDatum object
				var user = new UserDatum
				{
					FirstName = model.FirstName,
					LastName = model.LastName,
					UserName = model.UserName,
					Email = model.Email,
					Age = model.Age,
					Password = hashPassword,
					ProfileImage = profileImagePath // Save the image path in the database
				};

				// Save the new user to the database
				await _context.UserData.AddAsync(user);
				await _context.SaveChangesAsync();

				// Return success response
				return Ok(new { Success = true, Message = "Kullanıcı başarıyla kaydedildi." });
			}
			catch (Exception ex)
			{
				// Log the exception (optional, not shown here)
				return StatusCode(500, new { Success = false, ErrorMessage = $"Sunucu hatası: {ex.Message}" });
			}
		}
	}
}
