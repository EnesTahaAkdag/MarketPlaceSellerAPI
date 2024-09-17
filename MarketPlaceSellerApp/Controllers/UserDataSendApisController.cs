using MarketPlaceSellerApp.Models;
using MarketPlaceSellerApp.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace MarketPlaceSellerApp.Controllers
{
	[Authorize]
	[Route("[controller]")]
	[ApiController]
	public class UserDataSendApisController : ControllerBase
	{
		private readonly HepsiburadaSellerInformationContext _context;

		public UserDataSendApisController(HepsiburadaSellerInformationContext context)
		{
			_context = context;
		}

		[HttpGet("DataSend")]
		public async Task<IActionResult> DataSend(string userName)
		{
			if (string.IsNullOrEmpty(userName))
			{
				return BadRequest(new { Success = false, ErrorMessage = "Kullanıcı Adı Boş Olamaz" });
			}
			else
			{
				try
				{
					var user = await _context.UserData
					.Where(u => u.UserName == userName)
					.Select(u => new
					{
						u.FirstName,
						u.LastName,
						u.UserName,
						u.Email,
						u.Age,
						ProfileImageUrl = string.IsNullOrEmpty(u.ProfileImage)
							? null
							: $"{Request.Scheme}://{Request.Host}/images/{u.ProfileImage}"
					})
					.FirstOrDefaultAsync();

					if (user == null)
					{
						return NotFound(new
						{
							Success = false,
							ErrorMessage = "Kullanıcı Bulunamadı"
						});
					}
					else
					{
						return Ok(new
						{
							Success = true,
							Data = user
						});
					}
				}
				catch (Exception ex)
				{
					return StatusCode(500, new
					{
						Success = false,
						ErrorMessage = ex.Message
					});
				}
			}
		}




		[HttpGet("UserList")]
		public async Task<ActionResult> UserList()
		{
			try
			{
				int count = _context.UserData.Count();
				int totalCount = await _context.UserData.CountAsync();
				var data = await (from c in _context.UserData
								  orderby c.Id
								  select new UserList
								  {
									  Id = c.Id,
									  Email = c.Email,
									  FirstName = c.FirstName,
									  LastName = c.LastName,
									  UserName = c.UserName,
									  Age = c.Age,
								  })
								  .Take(count)
								  .ToListAsync();
				var response = new UserApiResponse
				{
					Success = true,
					ErrorMessage = null,
					Data = data,
					TotalCount = totalCount
				};
				var json = JsonConvert.SerializeObject(response, new JsonSerializerSettings() { DateFormatString = "dd-MM-yyyy" });

				return Content(json, "application/json");
			}
			catch (Exception ex)
			{
				var response = new UserApiResponse
				{
					Success = false,
					ErrorMessage = ex.Message,
				};
				return BadRequest(response);
			}
		}
	}
}
