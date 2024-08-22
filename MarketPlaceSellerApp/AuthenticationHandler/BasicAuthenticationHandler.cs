using MarketPlaceSellerApp.Helpers;
using MarketPlaceSellerApp.ViewModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;

namespace MarketPlaceSellerApp.AuthenticationHandler
{
	public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
	{
		private readonly AuthHelpers _authHelpers;
		public BasicAuthenticationHandler(
		IOptionsMonitor<AuthenticationSchemeOptions> options,
		ILoggerFactory logger,
		UrlEncoder encoder,
		ISystemClock clock,
		AuthHelpers authHelpers) : base(options, logger, encoder, clock)
		{
			_authHelpers = authHelpers;
		}

		protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
		{
			if (!Request.Headers.ContainsKey("Authorization"))
			{
				return AuthenticateResult.Fail("Missing Authorization Header");
			}

			try
			{
				var authHeader = AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]);
				var credentialBytes = Convert.FromBase64String(authHeader.Parameter);
				var credentials = Encoding.UTF8.GetString(credentialBytes).Split(':');
				var userName = credentials[0];
				var password = credentials[1];

				if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(password))
				{
					var loginUser = new LoginUser { UserName = userName, Password = password };
					var result = await _authHelpers.UserAuthentication(loginUser);
					if (result is OkObjectResult)
					{
						var claims = new[] {
						new Claim(ClaimTypes.NameIdentifier,userName),
						new Claim(ClaimTypes.Name,userName),
						};
						var identity = new ClaimsIdentity(claims, Scheme.Name);
						var principal = new ClaimsPrincipal(identity);
						var ticket = new AuthenticationTicket(principal, Scheme.Name);
						return AuthenticateResult.Success(ticket);
					}
					else
					{
						return AuthenticateResult.Fail("Kullanıcı Adı Veya Parola Yanlış");
					}
				}
				else
				{
					return AuthenticateResult.Fail("Geçersiz Kullanıcı Adı Veya Parola");
				}
			}
			catch
			{
				return AuthenticateResult.Fail("Geçersiz Yetkilendirme");
			}
		}
	}
}
