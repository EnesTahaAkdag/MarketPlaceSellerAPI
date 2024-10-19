using MarketPlaceSellerApp.HashingPassword;
using MarketPlaceSellerApp.Helpers;
using MarketPlaceSellerApp.Models;
using MarketPlaceSellerApp.ViewModel;
using MarketPlaceSellerApp.FileNameGuid;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Net.Mail;

namespace MarketPlaceSellerApp.Controllers
{
	[Route("[controller]")]
	[ApiController]
	public class RegisterAndLoginApiController : ControllerBase
	{
		private readonly HepsiburadaSellerInformationContext _context;
		private readonly AuthHelpers _authHelpers;
		private readonly GuidOperation _guidOperation;

		public RegisterAndLoginApiController(HepsiburadaSellerInformationContext context, AuthHelpers authHelpers, GuidOperation guidOperation)
		{
			_context = context;
			_authHelpers = authHelpers;
			_guidOperation = guidOperation;
		}

		[HttpPost("RegisterUser")]
		public async Task<IActionResult> RegisterUser([FromBody] User model)
		{
			if (!ModelState.IsValid)
				return BadRequest(new { Success = false, ErrorMessage = "Geçersiz model girdisi." });

			if (await _context.UserData.AnyAsync(m => m.UserName == model.UserName || m.Email == model.Email))
			{
				var errorMessage = await _context.UserData.AnyAsync(m => m.UserName == model.UserName)
					? "Kullanıcı adı zaten mevcut."
					: "Email adresi zaten kayıtlı.";
				return BadRequest(new { Success = false, ErrorMessage = errorMessage });
			}

			try
			{
				var hashPassword = HashingAndVerifyPassword.HashingPassword.HashPassword(model.Password);

				var profileImagePath = !string.IsNullOrEmpty(model.ProfileImageBase64)
					? await _guidOperation.SaveProfileImageAsync(model.ProfileImageBase64)
					: "profilephotots.png";

				var user = new UserDatum
				{
					FirstName = model.FirstName,
					LastName = model.LastName,
					UserName = model.UserName,
					Email = model.Email,
					Password = hashPassword,
					ProfileImage = profileImagePath,
					Age = !string.IsNullOrWhiteSpace(model.Age) ? Convert.ToDateTime(model.Age) : (DateTime?)null
				};

				await _context.UserData.AddAsync(user);
				await _context.SaveChangesAsync();

				return Ok(new { Success = true, Message = "Kullanıcı başarıyla kaydedildi." });
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { Success = false, ErrorMessage = $"Sunucu hatası: {ex.Message}" });
			}
		}

		[HttpPost("LoginUserData")]
		public async Task<IActionResult> LoginUserData([FromBody] LoginUser model)
		{
			if (model == null)
				return BadRequest(new { Success = false, ErrorMessage = "Boş Değer Gönderilemez" });

			try
			{
				var authResult = await _authHelpers.UserAuthentication(model);
				return authResult;
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { Success = false, ErrorMessage = $"Sunucu hatası: {ex.Message}" });
			}
		}

		[HttpPost("ForgetPassword")]
		public async Task<IActionResult> ForgetPassword([FromBody] ForgetPassword model)
		{
			if (model == null || string.IsNullOrEmpty(model.UserName))
				return BadRequest(new { Success = false, ErrorMessage = "Veri boş geldi" });

			var existingUser = await _context.UserData.FirstOrDefaultAsync(m => m.UserName == model.UserName);

			if (existingUser == null)
				return NotFound(new { Success = false, ErrorMessage = "Kullanıcı bulunamadı" });

			try
			{
				var randomCode = new Random().Next(100000, 999999).ToString();
				existingUser.ValidationCode = randomCode;

				_context.UserData.Update(existingUser);
				await _context.SaveChangesAsync();

				await SendEmailValidationCode(existingUser.Email, randomCode);

				return Ok(new { Success = true, Message = "Doğrulama kodu gönderildi." });
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { Success = false, ErrorMessage = $"Hata: {ex.Message}" });
			}
		}

		private async Task SendEmailValidationCode(string email, string validationCode)
		{
			string subject = "Doğrulama Kodu";
			string message = $"Doğrulama Kodunuz: {validationCode}";

			await SendEmail(email, subject, message);
		}

		private async Task SendEmail(string email, string subject, string message)
		{
			string smtpServer = "smtp.gmail.com";
			int smtpPort = 587;
			string smtpEmail = "akdagenestaha@gmail.com";
			string smtpPassword = Environment.GetEnvironmentVariable("aclb kead vwkg xvsm");

			using (var mailMessage = new MailMessage(smtpEmail, email, subject, message))
			{
				using (var smtpClient = new SmtpClient(smtpServer, smtpPort))
				{
					smtpClient.Credentials = new NetworkCredential(smtpEmail, smtpPassword);
					smtpClient.EnableSsl = true;

					await smtpClient.SendMailAsync(mailMessage);
				}
			}
		}

		[HttpPost("ValidateCode")]
		public async Task<IActionResult> ValidateCode([FromBody] VerificationCodeModel model)
		{
			if (model == null || string.IsNullOrEmpty(model.ValidationCode))
				return BadRequest(new { Success = false, ErrorMessage = "Veri boş veya hatalı." });

			var user = await _context.UserData.FirstOrDefaultAsync(m => m.UserName == model.UserName);

			if (user == null)
				return NotFound(new { Success = false, ErrorMessage = "Kullanıcı bulunamadı." });

			if (!user.ValidationCode.Equals(model.ValidationCode))
				return BadRequest(new { Success = false, ErrorMessage = "Doğrulama kodu yanlış." });

			return Ok(new { Success = true, Message = "Doğrulama başarılı." });
		}

		[HttpPost("ChangePassword")]
		public async Task<IActionResult> ChangePassword([FromBody] ChancePasswordModel model)
		{
			var user = await _context.UserData.AsNoTracking().FirstOrDefaultAsync(m => m.UserName == model.UserName);

			if (user == null)
				return BadRequest(new { Success = false, ErrorMessage = "Kullanıcı bulunamadı" });

			if (HashingAndVerifyPassword.HashingPassword.VerifyPassword(model.Password, user.Password))
				return BadRequest(new { Success = false, ErrorMessage = "Yeni şifre eski şifre ile aynı olamaz" });

			user.Password = HashingAndVerifyPassword.HashingPassword.HashPassword(model.Password);

			_context.UserData.Update(user);
			await _context.SaveChangesAsync();

			return Ok(new { Success = true, Message = "Şifre başarıyla güncellendi" });
		}
	}
}
