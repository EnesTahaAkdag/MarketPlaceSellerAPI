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
				return new BadRequestObjectResult(new { Success = false, Message = "Giriş bilgileri boş olamaz." });
			}

			if (string.IsNullOrWhiteSpace(model.UserName) || string.IsNullOrWhiteSpace(model.Password))
			{
				return new BadRequestObjectResult(new { Success = false, Message = "Kullanıcı adı ve parola boş olamaz." });
			}

			try
			{
				var dataCheck = await _context.UserData
					.AsNoTracking()
					.FirstOrDefaultAsync(m => m.UserName == model.UserName);

				if (dataCheck == null)
				{
					return new UnauthorizedObjectResult(new { Success = false, Message = "Kullanıcı bulunamadı." });
				}

				bool isPasswordValid = HashingAndVerifyPassword.HashingPassword.VerifyPassword(model.Password, dataCheck.Password);

				if (!isPasswordValid)
				{
					return new UnauthorizedObjectResult(new { Success = false, Message = "Kullanıcı adı veya parola hatalı!" });
				}

				return new OkObjectResult(new { Success = true, Message = "Kullanıcı girişi başarılı." });
			}
			catch (Exception ex)
			{
				return new ObjectResult(new { Success = false, Message = $"Bir hata oluştu: {ex.Message}" }) { StatusCode = 500 };
			}
		}
	}
}
