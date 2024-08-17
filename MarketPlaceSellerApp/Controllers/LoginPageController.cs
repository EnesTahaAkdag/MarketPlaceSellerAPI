using MarketPlaceSellerApp.Models;
using MarketPlaceSellerApp.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MarketPlaceSellerApp.HashingPassword;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;

namespace MarketPlaceSellerApp.Controllers
{
	[AllowAnonymous]
	[ApiController]
	[Route("[controller]")]
	public class LoginPageController : ControllerBase
	{
		private readonly HepsiburadaSellerInformationContext _context;

		public LoginPageController(HepsiburadaSellerInformationContext context)
		{
			_context = context;
		}
		[HttpPost("LoginUserData")]
		public async Task<IActionResult> LoginUserData([FromBody] LoginUser model)
		{
			if (model == null)
			{
				return BadRequest(new { Success = false, Message = "Geçersiz model" });
			}

			try
			{
				var dataCheck = await _context.UserData
					.AsNoTracking()
					.FirstOrDefaultAsync
					(m => m.UserName == model.UserName);
				if (dataCheck != null)
				{
					bool isPasswordValid = HashingAndVerifyPassword.HashingPassword.VerifyPassword(model.Password, dataCheck.Password);

					if (isPasswordValid)
					{
						//var tokenHadler = new JwtSecurityTokenHandler();
						//var key = Encoding.UTF8.GetBytes("eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyfQcThIIoDvwdueQB468K5xDc5633seEFoqwxjFxSJyQQ");
						//var tokenDescriptor = new SecurityTokenDescriptor
						//{
						//	Subject = new ClaimsIdentity(new[]
						//	{
						//		new Claim(ClaimTypes.Name,dataCheck.UserName),
						//	}),
						//	Expires = DateTime.UtcNow.AddHours(1),
						//	SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
						//};
						//var token = tokenHadler.CreateToken(tokenDescriptor);
						//var tokenString = tokenHadler.WriteToken(token);
						return Ok(new { Success = true, Message = "Kullanıcı Girişi Başarılı" });
					}
					else
					{
						return Unauthorized(new { Success = false, Message = "Kullanıcı Girişi Başarısız" });
					}
				}
				else
				{
					return Unauthorized(new { Success = false, Message = "Kullanıcı Bulunamadı" });
				}
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { Success = false, ErrorMessage = ex.Message });
			}
		}
	}
}
