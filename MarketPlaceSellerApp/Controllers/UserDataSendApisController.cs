using MarketPlaceSellerApp.Models;
using MarketPlaceSellerApp.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace MarketPlaceSellerApp.Controllers
{
	[Authorize]
	[Route("[controller]")]
	[ApiController]
	public class UserDataSendApiController : ControllerBase
	{
		private readonly HepsiburadaSellerInformationContext _context;

		public UserDataSendApiController(HepsiburadaSellerInformationContext context)
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

			try
			{
				var user = await _context.UserData
					.Where(u => u.UserName == userName)
					.AsNoTracking()
					.Select(u => new
					{
						u.FirstName,
						u.LastName,
						u.UserName,
						u.Email,
						u.Age,
						ProfileImageBase64 = string.IsNullOrEmpty(u.ProfileImage)
							? null
							: $"https://be65-37-130-115-91.ngrok-free.app/profile_images/{u.ProfileImage}"
					})
					.FirstOrDefaultAsync();

				if (user == null)
				{
					return NotFound(new { Success = false, ErrorMessage = "Kullanıcı Bulunamadı" });
				}

				return Ok(new { Success = true, Data = user });
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { Success = false, ErrorMessage = "Hata bir oluştu.", Details = ex.Message });
			}
		}




		[HttpGet("UserList")]
		public async Task<ActionResult> UserList(int? page,int? pageSize)
		{
			try
			{
				int totalCount = await _context.UserData.CountAsync();
				int currentPage = page ?? 1;
				pageSize = pageSize ?? 1;
				int totalPage = (int)Math.Ceiling((decimal)totalCount / pageSize.GetValueOrDefault());

				var data = await (from c in _context.UserData
								  orderby c.Id
								  select new UserList
								  {
									  Id = c.Id,
									  Email = c.Email,
									  FirstName = c.FirstName,
									  LastName = c.LastName,
									  UserName = c.UserName,
									  Age = c.Age.GetValueOrDefault().ToString("yyyy-MM-dd"),
								  })
								  .Skip((currentPage - 1)* pageSize.GetValueOrDefault())
								  .Take(pageSize.GetValueOrDefault())
								  .AsNoTracking()
								  .ToListAsync();
				var response = new UserApiResponse
				{
					Success = true,
					ErrorMessage = null,
					Data = data,
					TotalCount = totalCount
				};
				var json = JsonConvert.SerializeObject(response, new JsonSerializerSettings() { DateFormatString = "dd-MM-yyyy" });

				return Ok(new ApiResponse
				{
					Success = true,
					ErrorMessage = null,
					Data = data,
					Page =currentPage,
					PageSize = pageSize.GetValueOrDefault(),
					TotalCount = totalCount,
					TotalPage=totalPage
				});
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
