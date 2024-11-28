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
		private readonly ILogger<BasicAuthenticationHandler> _logger;

		public BasicAuthenticationHandler(
			IOptionsMonitor<AuthenticationSchemeOptions> options,
			ILoggerFactory loggerFactory,
			UrlEncoder encoder,
			AuthHelpers authHelpers) : base(options, loggerFactory, encoder)
		{
			_authHelpers = authHelpers;
			_logger = loggerFactory.CreateLogger<BasicAuthenticationHandler>();
		}

		protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
		{
			if (!Request.Headers.ContainsKey("Authorization"))
			{
				_logger.LogWarning("Authorization header is missing");
				return AuthenticateResult.Fail("Authorization header is missing. Please provide valid credentials.");
			}

			try
			{
				var authHeader = AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]);
				var credentialBytes = Convert.FromBase64String(authHeader.Parameter);
				var credentials = Encoding.UTF8.GetString(credentialBytes).Split(':');
				var userName = credentials[0];
				var password = credentials[1];

				if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
				{
					_logger.LogWarning("Empty username or password received");
					return AuthenticateResult.Fail("Username or password cannot be empty.");
				}

				var loginUser = new LoginUser { UserName = userName, Password = password };
				var result = await _authHelpers.UserAuthentication(loginUser);

				if (result is OkObjectResult)
				{
					_logger.LogInformation($"User '{userName}' authenticated successfully");
					var claims = new[] {
						new Claim(ClaimTypes.NameIdentifier, userName),
						new Claim(ClaimTypes.Name, userName),
					};
					var identity = new ClaimsIdentity(claims, Scheme.Name);
					var principal = new ClaimsPrincipal(identity);
					var ticket = new AuthenticationTicket(principal, Scheme.Name);
					return AuthenticateResult.Success(ticket);
				}
				else
				{
					_logger.LogWarning($"Invalid credentials for user '{userName}'");
					return AuthenticateResult.Fail("Invalid username or password. Please try again.");
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occurred during authentication");
				return AuthenticateResult.Fail("An error occurred during authentication. Please contact support.");
			}
		}
	}
}
