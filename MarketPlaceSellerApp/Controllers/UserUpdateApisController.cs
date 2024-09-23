using MarketPlaceSellerApp.HashingPassword;
using MarketPlaceSellerApp.Models;
using MarketPlaceSellerApp.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;

namespace MarketPlaceSellerApp.Controllers
{
	[Authorize]
	[Route("[controller]")]
	[ApiController]
	public class UserUpdateApisController : ControllerBase
	{
		private readonly HepsiburadaSellerInformationContext _context;
		private readonly IWebHostEnvironment _environment;

		public UserUpdateApisController(HepsiburadaSellerInformationContext context, IWebHostEnvironment environment)
		{
			_context = context;
			_environment = environment;
		}



		[HttpPost("UpdatePassword")]
		public async Task<IActionResult> UpdatePassword([FromBody] UpdatePassword model)
		{
			try
			{
				var user = await _context.UserData.FirstOrDefaultAsync(m => m.UserName == model.UserName);

				if (user != null)
				{
					string hashPassword = HashingAndVerifyPassword.HashingPassword.HashPassword(model.Password);
					user.Password = hashPassword;

					_context.UserData.Update(user);
					await _context.SaveChangesAsync();
					return Ok(new { Success = true, Message = "Şifre Başarıyla Güncellendi" });
				}
				else
				{
					return BadRequest(new { Success = false, ErrorMessage = "Kullanıcı Bulunamadı" });
				}
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { Success = false, ErrorMessage = ex.Message });
			}
		}



		[HttpPost("UpdateProfileImage")]
		public async Task<IActionResult> UpdateProfilePhoto([FromForm] UpdateProfilePhoto model)
		{
			try
			{
				// Kullanıcıyı veritabanında bul
				var user = await _context.UserData.FirstOrDefaultAsync(m => m.UserName == model.UserName);

				if (user != null)
				{
					// Eğer resim dosyası gönderildiyse işle
					if (model.ProfileImage != null)
					{
						var fileName = $"{Guid.NewGuid().ToString("N")}.jpg"; // Benzersiz dosya adı oluştur
						var filePath = Path.Combine(_environment.ContentRootPath, "wwwroot", "profile_images", fileName); // Dosya yolu

						Directory.CreateDirectory(Path.GetDirectoryName(filePath)); // Klasör yoksa oluştur

						// Dosya sistemine resim dosyasını kaydet
						await using (var stream = new FileStream(filePath, FileMode.Create))
						{
							await model.ProfileImage.CopyToAsync(stream);
						}

						// Kullanıcının profil resmini güncelle
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
		[HttpPost("UpdateUserProfileImage")]
		public async Task<IActionResult> UpdateUserProfilePhoto([FromForm] UpdateProfilePhoto model)
		{
			try
			{
				var user = await _context.UserData.FirstOrDefaultAsync(m => m.UserName == model.UserName);

				if (user != null)
				{
					if (model.ProfileImage != null)
					{
						var fileName = $"{Guid.NewGuid().ToString("N")}.jpg";
						var filePath = Path.Combine(_environment.ContentRootPath, "wwwroot", "profile_images", fileName);

						Directory.CreateDirectory(Path.GetDirectoryName(filePath));

						await using (var stream = new FileStream(filePath, FileMode.Create))
						{
							await model.ProfileImage.CopyToAsync(stream);
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






		[HttpPost("EditUserData")]
		public async Task<IActionResult> UpdateUserData([FromBody] UpdateUser model)
		{
			try
			{
				var user = await _context.UserData.FirstOrDefaultAsync(m => m.UserName == model.UserName);

				if (user != null)
				{
					// Sadece değişen alanları güncelleyin
					user.FirstName = model.FirstName;
					user.LastName = model.LastName;
					user.Email = model.Email;
					user.Age = model.Age;

					_context.UserData.Update(user);
					await _context.SaveChangesAsync();

					return Ok(new { Success = true, Message = "Kullanıcı Bilgileri Başarıyla Güncellendi" });
				}
				else
				{
					return BadRequest(new { Success = false, ErrorMessage = "Kullanıcı Bulunamadı" });
				}
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { Success = false, ErrorMessage = ex.Message });
			}
		}

	}
}
