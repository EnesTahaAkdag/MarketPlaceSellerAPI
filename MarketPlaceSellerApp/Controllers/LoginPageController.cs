using MarketPlaceSellerApp.ViewModel;
using MarketPlaceSellerApp.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace MarketPlaceSellerApp.Controllers
{
	[AllowAnonymous]
	[ApiController]
	[Route("[controller]")]
	public class LoginPageController : ControllerBase
	{
		private readonly AuthHelpers _authHelpers;
		public LoginPageController(AuthHelpers authHelpers)
		{
			_authHelpers = authHelpers;
		}
		[HttpPost("LoginUserData")]
		public async Task<IActionResult> LoginUserData([FromBody] LoginUser model)
		{
			if (model == null)
			{
				return BadRequest(new { Success = false, Message = "Boş Değer Gönderilemez" });
			}
			try
			{
				var authResult = await _authHelpers.UserAuthentication(model);
				if (authResult is OkObjectResult)
				{
					return authResult;
				}
				else
				{
					return authResult;
				}
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { Success = false, ErrorMessage = ex.Message });
			}
		}
	}
}
