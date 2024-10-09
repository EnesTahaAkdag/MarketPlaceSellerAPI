using MarketPlaceSellerApp.HashingPassword;
using MarketPlaceSellerApp.Helpers;
using MarketPlaceSellerApp.Models;
using MarketPlaceSellerApp.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Diagnostics.Contracts;
using System.Drawing.Text;
using System.Net;
using System.Net.Mail;

namespace MarketPlaceSellerApp.Controllers
{
	[Route("[controller]")]
	[ApiController]
	public class RegisterAndLoginApiController : ControllerBase
	{
		public readonly HepsiburadaSellerInformationContext _context;
		private readonly AuthHelpers _authHelpers;
		private readonly IWebHostEnvironment _webHostEnvironment;

		public RegisterAndLoginApiController(HepsiburadaSellerInformationContext context, AuthHelpers authHelpers, IWebHostEnvironment webHostEnvironment)
		{
			_context = context;
			_authHelpers = authHelpers;
			_webHostEnvironment = webHostEnvironment;
		}

		[HttpPost("RegisterUser")]
		public async Task<IActionResult> RegisterUser([FromForm] User model)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(new { Success = false, ErrorMessage = "Geçersiz model girdisi." });
			}

			try
			{
				var existingUser = await _context.UserData
					.FirstOrDefaultAsync(m => m.UserName == model.UserName
					|| m.Email == model.Email);

				if (existingUser != null)
				{
					var errorMessage = existingUser.UserName == model.UserName
						? "Kullanıcı adı zaten mevcut."
						: "Email adresi zaten kayıtlı.";
					return BadRequest(new { Success = false, ErrorMessage = errorMessage });
				}

				string hashPassword = HashingAndVerifyPassword.HashingPassword.HashPassword(model.Password);

				string profileImagePath = null;

				if (model.ProfileImage != null)
				{
					profileImagePath = await SaveProfileImageAsync(model.ProfileImage);
				}

				var user = new UserDatum
				{
					FirstName = model.FirstName,
					LastName = model.LastName,
					UserName = model.UserName,
					Email = model.Email,
					Password = hashPassword,
					ProfileImage = profileImagePath
				};

				if (!string.IsNullOrWhiteSpace(model.Age))
				{
					user.Age = Convert.ToDateTime(model.Age);
				}

				_context.UserData.Add(user);
				await _context.SaveChangesAsync();

				return Ok(new { Success = true, Message = "Kullanıcı başarıyla kaydedisldi." });
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { Success = false, ErrorMessage = $"Sunucu hatası: {ex.Message}" });
			}
		}


		private async Task<string> SaveProfileImageAsync(IFormFile profileImageBytes)
		{
			try
			{
				var fileName = $"{Guid.NewGuid().ToString("N")}.jpg";
				var filePath = Path.Combine(_webHostEnvironment.WebRootPath, "profile_images");

				Directory.CreateDirectory(filePath);

				var fullFilePath = Path.Combine(filePath, fileName);

				using (var fileStream = new FileStream(fullFilePath, FileMode.Create))
				{
					await profileImageBytes.CopyToAsync(fileStream);
				}
				return fileName;
			}
			catch (Exception ex)
			{
				throw new Exception($"Profil resmi yüklenirken hata oluştu: {ex.Message}");
			}
		}






		[HttpPost("LoginUserData")]
		public async Task<IActionResult> LoginUserData([FromBody] LoginUser model)
		{
			if (model == null)
			{
				return BadRequest(new { Success = false, ErrorMessage = "Boş Değer Gönderilemez" });
			}

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
		public async Task<ActionResult> ForgetPassword([FromBody] ForgetPassword model)
		{
			if (model == null || string.IsNullOrEmpty(model.UserName))
			{
				return BadRequest(new { Success = false, ErrorMessage = "Veri boş geldi" });
			}

			try
			{
				var existingUser = await _context.UserData
					.FirstOrDefaultAsync(m => m.UserName == model.UserName);

				if (existingUser != null)
				{
					Random r = new Random();
					int randomCode = r.Next(100000, 999999);

					existingUser.ValidationCode = randomCode.ToString();

					_context.UserData.Update(existingUser);
					await _context.SaveChangesAsync();

					await SendEmailValidationCode(existingUser.UserName, randomCode);

					return Ok(new { Success = true, ErrorMessage = "E-posta doğrulandı, Mail gönderildi" });
				}
				else
				{
					return NotFound(new { Success = false, ErrorMessage = "E-posta doğrulanamadı" });
				}
			}
			catch (Exception ex)
			{
				return BadRequest(new { Success = false, ErrorMessage = $"Hata Oluştu: {ex.Message}" });
			}
		}


		private async Task SendEmailValidationCode(string UserName, int validationCode)
		{
			var user = await _context.UserData.FirstOrDefaultAsync(u => u.UserName == UserName);

			if (user != null)
			{
				string email = user.Email;

				string subject = "Doğrulama Kodu";
				string message = $"Doğrulama Kodunuz: {validationCode}";

				await SendEmail(email, subject, message);
			}
			else
			{
				throw new Exception("Mail Adresi Bulunamadı");
			}
		}


		private async Task SendEmail(string email, string subject, string message)
		{
			string smtpServer = "smtp.gmail.com";
			int smtpPort = 587;
			string smtpEmail = "akdagenestaha@gmail.com";
			string smtpPassword = "aclb kead vwkg xvsm";

			MailMessage mailMessage = new MailMessage
			{
				From = new MailAddress(smtpEmail),
				Subject = subject,
				Body = message,
				IsBodyHtml = false
			};

			mailMessage.To.Add(email);

			using (SmtpClient smtpClient = new SmtpClient(smtpServer, smtpPort))
			{
				smtpClient.Credentials = new NetworkCredential(smtpEmail, smtpPassword);
				smtpClient.EnableSsl = true;

				await smtpClient.SendMailAsync(mailMessage);
			}

			[HttpPost("VerifyCode")]
			public async Task<ActionResult> VerifyCode([FromBody] VerificationCodeModel model)
			{
				if (model == null || model.VerificationCode > 0)
				{
					return BadRequest(new { Success = false, ErrorMessage = "Veri Boş Veya Hatalı Geldi" });
				}

				try
				{
					var existingCode = await _context.UserData.FirstOrDefaultAsync(m => m.ValidationCode = model.VerificationCode);
				}
				catch (Exception ex)
				{
					return BadRequest(new { Success = false, ErrorMessage = $"Hata Oluştu: {ex.Message}" });
				}
			}
		}
	}
}
