//using Microsoft.AspNetCore.Authentication;
//using Microsoft.Extensions.Logging;
//using Microsoft.Extensions.Options;
//using System.Net.Http.Headers;
//using System.Security.Claims;
//using System.Text;
//using System.Text.Encodings.Web;
//using System.Threading.Tasks;


//namespace MarketPlaceSellerApp.AuthenticationHandler
//{
//	public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
//	{
//		public BasicAuthenticationHandler(
//		IOptionsMonitor<AuthenticationSchemeOptions> options,
//		ILoggerFactory logger,
//		UrlEncoder encoder,
//		ISystemClock clock) : base(options, logger, encoder, clock)
//		{
//		}

//		protected override Task<AuthenticateResult> HandleAuthenticateAsync()
//		{
//			if (!Request.Headers.ContainsKey("Authorization"))
//			{
//				return Task.FromResult(AuthenticateResult.Fail("Missing Authorization Header"));
//			}

//			try
//			{
//				var authHeader = AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]);
//				var credentialBytes = Convert.FromBase64String(authHeader.Parameter);
//				var credentials = Encoding.UTF8.GetString(credentialBytes).Split(':');
//				var userName = credentials[0];
//				var password = credentials[1];

//				if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(password))
//				{
//					if (userName == "admin" && password == "123")
//					{
//						var claims = new[]
//						{
//							new Claim(ClaimTypes.NameIdentifier, userName),
//							new Claim(ClaimTypes.Name, userName),
//						};
//						var identity = new ClaimsIdentity(claims, Scheme.Name);
//						var principal = new ClaimsPrincipal(identity);
//						var ticket = new AuthenticationTicket(principal, Scheme.Name);

//						return Task.FromResult(AuthenticateResult.Success(ticket));
//					}
//					else
//					{
//						return Task.FromResult(AuthenticateResult.Fail("Kullanıcı Adı veya Parola Hatalı"));
//					}
//				}
//				else
//				{
//					return Task.FromResult(AuthenticateResult.Fail("Geçersiz Kullanıcı Adı veya Parola"));
//				}
//			}
//			catch
//			{
//				return Task.FromResult(AuthenticateResult.Fail("Geçersiz Yetkilendirme"));
//			}
//		}
//	}
//}
