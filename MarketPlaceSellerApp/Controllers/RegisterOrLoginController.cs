﻿using MarketPlaceSellerApp.HashingPassword;
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
		private readonly IWebHostEnvironment _webHostEnvironment;

		public RegisterAndLoginApiController(HepsiburadaSellerInformationContext context, AuthHelpers authHelpers, GuidOperation guidOperation, IWebHostEnvironment webHostEnvironment)
		{
			_context = context;
			_authHelpers = authHelpers;
			_guidOperation = guidOperation;
			_webHostEnvironment = webHostEnvironment;
		}

		[HttpPost("RegisterUser")]
		public async Task<IActionResult> RegisterUser([FromBody] User model)
		{
			if (!ModelState.IsValid)
			{
				var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
				return BadRequest(new { Success = false, ErrorMessage = "Geçersiz model girdisi. Lütfen bilgileri eksiksiz ve doğru doldurun.", Errors = errors });
			}

			var existingUser = await _context.UserData
				.AsNoTracking()
				.Where(m => m.UserName == model.UserName || m.Email == model.Email)
				.FirstOrDefaultAsync();

			if (existingUser != null)
			{
				var errorMessage = existingUser.UserName == model.UserName
					? "Bu kullanıcı adı zaten kullanılmaktadır."
					: "Bu e-posta adresi ile zaten bir hesap bulunmaktadır.";
				return BadRequest(new { Success = false, ErrorMessage = errorMessage });
			}

			string profileImagePath = string.Empty;

			try
			{
				var hashPassword = HashingAndVerifyPassword.HashingPassword.HashPassword(model.Password);

				if (!string.IsNullOrEmpty(model.ProfileImageBase64) && model.ProfileImageBase64 != "profilephotots.png")
				{
					try
					{
						profileImagePath = await _guidOperation.SaveProfileImageAsync(model.ProfileImageBase64);
					}
					catch (InvalidOperationException ex)
					{
						return BadRequest(new { Success = false, ErrorMessage = $"Profil resmi kaydedilemedi: {ex.Message}" });
					}
				}

				var user = new UserDatum
				{
					FirstName = model.FirstName,
					LastName = model.LastName,
					UserName = model.UserName,
					Email = model.Email,
					Password = hashPassword,
					ProfileImage = profileImagePath,
				};
				await _context.AddAsync(user);
				await _context.SaveChangesAsync();

				return Ok(new { Success = true, Message = "Kayıt başarılı! Hesabınız oluşturuldu." });
			}
			catch (Exception ex)
			{
				if (!string.IsNullOrEmpty(profileImagePath) && profileImagePath != "profilephotots.png")
				{
					var fullFilePath = Path.Combine(_webHostEnvironment.WebRootPath, "profile_images", profileImagePath);
					if (System.IO.File.Exists(fullFilePath))
					{
						System.IO.File.Delete(fullFilePath);
					}
				}

				return StatusCode(500, new { Success = false, ErrorMessage = $"Sunucu hatası: {ex.Message}" });
			}
		}

		[HttpPost("LoginUserData")]
		public async Task<IActionResult> LoginUserData([FromBody] LoginUser model)
		{
			if (model == null || string.IsNullOrWhiteSpace(model.UserName) || string.IsNullOrWhiteSpace(model.Password))
			{
				return BadRequest(new { Success = false, ErrorMessage = "Kullanıcı adı ve parola alanları boş bırakılamaz." });
			}

			try
			{
				var authResult = await _authHelpers.UserAuthentication(model);
				return authResult;
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { Success = false, ErrorMessage = $"Sunucu hatası oluştu: {ex.Message}" });
			}
		}

		[HttpPost("ForgetPassword")]
		public async Task<IActionResult> ForgetPassword([FromBody] ForgetPassword model)
		{
			if (model == null || string.IsNullOrEmpty(model.UserName))
				return BadRequest(new { Success = false, ErrorMessage = "Kullanıcı adı sağlanmalıdır." });

			var existingUser = await _context.UserData.AsNoTracking().FirstOrDefaultAsync(m => m.UserName == model.UserName);

			if (existingUser == null)
				return NotFound(new { Success = false, ErrorMessage = "Kullanıcı bulunamadı." });

			try
			{
				var randomCode = new Random().Next(100000, 999999).ToString();
				existingUser.ValidationCode = randomCode;

				_context.UserData.Update(existingUser);
				await _context.SaveChangesAsync();

				await SendEmailValidationCode(existingUser.Email, randomCode);

				return Ok(new { Success = true, Message = "Doğrulama kodu başarıyla gönderildi. Lütfen e-posta adresinizi kontrol edin." });
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
			string smtpPassword = "xvyv xlfn hpqu rbko";

			using (var mailMessage = new MailMessage(smtpEmail, email, subject, message))
			{
				using (var smtpClient = new SmtpClient(smtpServer, smtpPort))
				{
					smtpClient.Credentials = new NetworkCredential(smtpEmail, smtpPassword);
					smtpClient.EnableSsl = true;

					try
					{
						await smtpClient.SendMailAsync(mailMessage);
					}
					catch (Exception ex)
					{
						Console.WriteLine("Error sending email: " + ex.Message);
						throw;
					}
				}
			}
		}

		[HttpPost("ValidateCode")]
		public async Task<IActionResult> ValidateCode([FromBody] VerificationCodeModel model)
		{
			if (model == null || string.IsNullOrEmpty(model.ValidationCode))
				return BadRequest(new { Success = false, ErrorMessage = "Doğrulama kodu boş veya hatalı." });

			var user = await _context.UserData.AsNoTracking().FirstOrDefaultAsync(m => m.UserName == model.UserName);

			if (user == null)
				return NotFound(new { Success = false, ErrorMessage = "Kullanıcı bulunamadı." });

			if (!user.ValidationCode.Equals(model.ValidationCode))
				return BadRequest(new { Success = false, ErrorMessage = "Doğrulama kodu yanlış." });

			return Ok(new { Success = true, Message = "Doğrulama başarılı!" });
		}

		[HttpPost("ChangePassword")]
		public async Task<IActionResult> ChangePassword([FromBody] ChancePasswordModel model)
		{
			if (model == null || string.IsNullOrEmpty(model.UserName) || string.IsNullOrEmpty(model.Password))
				return BadRequest(new { Success = false, ErrorMessage = "Kullanıcı adı ve yeni şifre boş olamaz." });

			var user = await _context.UserData.AsNoTracking().FirstOrDefaultAsync(m => m.UserName == model.UserName);

			if (user == null)
				return NotFound(new { Success = false, ErrorMessage = "Kullanıcı bulunamadı." });

			if (HashingAndVerifyPassword.HashingPassword.VerifyPassword(model.Password, user.Password))
				return BadRequest(new { Success = false, ErrorMessage = "Yeni şifre eski şifre ile aynı olamaz." });

			user.Password = HashingAndVerifyPassword.HashingPassword.HashPassword(model.Password);

			_context.UserData.Update(user);
			await _context.SaveChangesAsync();

			return Ok(new { Success = true, Message = "Şifre başarıyla güncellendi." });
		}
	}
}
