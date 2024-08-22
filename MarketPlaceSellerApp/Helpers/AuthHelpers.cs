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
			var dataCheck = await _context.UserData
					.AsNoTracking()
					.FirstOrDefaultAsync(m => m.UserName == model.UserName);

			if (dataCheck != null)
			{
				bool isPasswordValid = HashingAndVerifyPassword.HashingPassword.VerifyPassword(model.Password, dataCheck.Password);

				if (isPasswordValid)
				{
					return new OkObjectResult(new { Success = true, Message = "Kullanıcı Girişi Başarılı" });
				}
				else
				{
					return new UnauthorizedObjectResult(new { Success = false, Message = "Kullanıcı Girişi Başarısız" });
				}
			}
			else
			{
				return new UnauthorizedObjectResult(new { Success = false, Message = "Kullanıcı Bulunamadı" });
			}
		}
	}
}
