using MarketPlaceSellerApp.Helpers;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;


namespace MarketPlaceSellerApp.ViewModel
{
	public class LoginDataViewModel
	{
		[StringLength(50, ErrorMessage = "50 Karakterden Fazla Giriş Yapılamaz")]
		[Required(ErrorMessage = "İsim Boş Bırakılamaz")]
		public string FirstName { get; set; }

		[StringLength(50, ErrorMessage = "50 Karakterden Fazla Giriş Yapılamaz")]
		[Required(ErrorMessage = "Soyisim Boş Bırakılamaz")]
		public string LastName { get; set; }

		[StringLength(50, ErrorMessage = "50 Karakterden Fazla Giriş Yapılamaz")]
		[Required(ErrorMessage = "Kullanıcı Adı Boş Bırakılamaz")]
		public string UserName { get; set; }

		[StringLength(50, ErrorMessage = "50 Karakterden Fazla Giriş Yapılamaz")]
		[Required(ErrorMessage = "Email Boş Bırakılamaz")]
		[EmailAddress(ErrorMessage = "Geçerli bir email adresi giriniz.")]
		public string Email { get; set; }

		public DateTime? Age { get; set; }

		[DataType(DataType.Password)]
		[Required(ErrorMessage = "Lütfen Şifre Giriniz")]
		[RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])[a-zA-Z\d]{8,}$", ErrorMessage = "En az 8 karakterlik bir parola büyük ve küçük harflerin bir kombinasyonunu içermelidir.")]
		public string Password { get; set; }

		[DataType(DataType.Password)]
		[Required(ErrorMessage = "Lütfen Şifreyi Doğrulayınız")]
		[Compare("Password", ErrorMessage = "Şifreler uyuşmuyor.")]
		public string VerifyPassword { get; set; }
	}

	public class RegisterDataViewModel : LoginDataViewModel { }

	public class UpdateLoginDataViewModel : LoginDataViewModel
	{ public long Id { get; set; } }

	public class UserListViewModel
	{
		public long Id { get; set; }
		public string Email { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string UserName { get; set; }
		public DateTime? Age { get; set; }
	}

	public class APIResponse
	{
		public bool Success { get; set; }
		public string ErrorMessage { get; set; }
		public object Data { get; set; }
		public int TotalCount { get; set; }
	}
}