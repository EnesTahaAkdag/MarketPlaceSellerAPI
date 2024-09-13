using MarketPlaceSellerApp.Models;
using MarketPlaceSellerApp.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IO;

namespace MarketPlaceSellerApp.Controllers
{
	[Route("[controller]")]
	[ApiController]
	public class UserUpdateProfileImageAPIController : Controller
	{
		private readonly HepsiburadaSellerInformationContext _context;
		private readonly IWebHostEnvironment _environment;

		public UserUpdateProfileImageAPIController(HepsiburadaSellerInformationContext context, IWebHostEnvironment environment)
		{
			_context = context;
			_environment = environment;
		}

		[HttpPost("UpdateProfileImage")]
		public async Task<IActionResult> UpdateProfilePhoto([FromForm] UpdateProfilePhoto model)
		{
			try
			{
				var user = await _context.UserData.FirstOrDefaultAsync(m => m.UserName == model.UserName);

				if (user != null)
				{
					if (model.ProfilePhoto != null && model.ProfilePhoto.Length > 0)
					{
						if (string.IsNullOrEmpty(_environment.ContentRootPath))
						{
							return StatusCode(500, new
							{
								Success = false,
								ErrorMessage = "Web kök dizini bulunamadı."
							});
						}

						var fileName = $"{Guid.NewGuid()}{Path.GetExtension(model.ProfilePhoto.FileName)}";
						var filePath = fileName;

						if (string.IsNullOrEmpty(Path.GetDirectoryName(filePath)))
						{
							return StatusCode(500, new
							{
								Success = false,
								ErrorMessage = "Dosya yolu oluşturulamadı."
							});
						}

						Directory.CreateDirectory(Path.GetDirectoryName(filePath));

						using (var stream = new FileStream(filePath, FileMode.Create))
						{
							await model.ProfilePhoto.CopyToAsync(stream);
						}

						user.ProfileImage = fileName;
						_context.Update(user);
						await _context.SaveChangesAsync();

						return Ok(new
						{
							Success = true,
							Message = "Profil Resmi Başarıyla Güncellendi"
						});
					}
					else
					{
						return BadRequest(new
						{
							Success = false,
							ErrorMessage = "Geçerli bir resim dosyası yükleyin."
						});
					}
				}
				else
				{
					return BadRequest(new
					{
						Success = false,
						ErrorMessage = "Kullanıcı Bulunamadı."
					});
				}
			}
			catch (Exception ex)
			{
				return StatusCode(500, new
				{
					Success = false,
					ErrorMessage = ex.Message
				});
			}
		}
	}
}
