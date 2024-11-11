using MarketPlaceSellerApp.HashingPassword;
using MarketPlaceSellerApp.Models;
using MarketPlaceSellerApp.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MarketPlaceSellerApp.Helpers
{
	public class AuthHelpers
	{
		private readonly HepsiburadaSellerInformationContext _context;

		public AuthHelpers(HepsiburadaSellerInformationContext context)
		{
			_context = context;
		}

		public async Task<IActionResult> UserAuthentication(LoginUser model)
		{
			if (model == null)
			{
				return new BadRequestObjectResult(new
				{
					Success = false,
					Message = "Giriş bilgileri boş olamaz.",
					UserGuidance = "Lütfen kullanıcı adı ve şifrenizi girin."
				});
			}

			if (string.IsNullOrWhiteSpace(model.UserName) || string.IsNullOrWhiteSpace(model.Password))
			{
				return new BadRequestObjectResult(new
				{
					Success = false,
					Message = "Kullanıcı adı ve parola boş olamaz.",
					UserGuidance = "Lütfen geçerli bir kullanıcı adı ve şifre girin."
				});
			}

			try
			{
				var dataCheck = await _context.UserData
					.AsNoTracking()
					.FirstOrDefaultAsync(m => m.UserName == model.UserName);

				if (dataCheck == null)
				{
					return new UnauthorizedObjectResult(new
					{
						Success = false,
						Message = "Kullanıcı bulunamadı.",
						UserGuidance = "Lütfen doğru kullanıcı adını girin veya hesabınızı kontrol edin."
					});
				}

				bool isPasswordValid = HashingAndVerifyPassword.HashingPassword.VerifyPassword(model.Password, dataCheck.Password);

				if (!isPasswordValid)
				{
					return new UnauthorizedObjectResult(new
					{
						Success = false,
						Message = "Kullanıcı adı veya parola hatalı!",
						UserGuidance = "Lütfen kullanıcı adınızı ve şifrenizi kontrol edin."
					});
				}

				return new OkObjectResult(new
				{
					Success = true,
					Message = "Kullanıcı girişi başarılı.",
					UserGuidance = "Hoş geldiniz! Ana sayfaya yönlendiriliyorsunuz."
				});
			}
			catch (Exception ex)
			{
				return new ObjectResult(new
				{
					Success = false,
					Message = "Bir hata oluştu. Lütfen tekrar deneyin veya destek ekibine başvurun.",
					Details = ex.Message,
					UserGuidance = "Giriş işlemi sırasında bir hata oluştu. Tekrar deneyin veya destek alın."
				})
				{
					StatusCode = 500
				};
			}
		}
	}
}
