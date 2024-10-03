using System.ComponentModel.DataAnnotations;

namespace MarketPlaceSellerApp.ViewModel
{
	public class User
	{
		[StringLength(50, ErrorMessage = "50 karakterden fazla giriş yapılamaz.")]
		[Required(ErrorMessage = "İsim boş bırakılamaz.")]
		public string FirstName { get; set; }

		[StringLength(50, ErrorMessage = "50 karakterden fazla giriş yapılamaz.")]
		[Required(ErrorMessage = "Soyisim boş bırakılamaz.")]
		public string LastName { get; set; }

		[StringLength(50, ErrorMessage = "50 karakterden fazla giriş yapılamaz.")]
		[Required(ErrorMessage = "Kullanıcı adı boş bırakılamaz.")]
		public string UserName { get; set; }

		[StringLength(50, ErrorMessage = "50 karakterden fazla giriş yapılamaz.")]
		[Required(ErrorMessage = "Email boş bırakılamaz.")]
		[EmailAddress(ErrorMessage = "Geçerli bir email adresi giriniz.")]
		public string Email { get; set; }

		public string Age { get; set; }

		[DataType(DataType.Password)]
		[Required(ErrorMessage = "Lütfen şifre giriniz.")]
		[RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])[a-zA-Z\d]{8,}$", ErrorMessage = "En az 8 karakterlik bir parola büyük ve küçük harflerin bir kombinasyonunu içermelidir.")]
		public string Password { get; set; }

		public string ProfileImage { get; set; }
	}


	public class LoginUser
    {
        [StringLength(50, ErrorMessage = "50 karakterden fazla giriş yapılamaz.")]
        [Required(ErrorMessage = "Kullanıcı adı boş bırakılamaz.")]
        public string UserName { get; set; }

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Lütfen şifre giriniz.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])[a-zA-Z\d]{8,}$", ErrorMessage = "En az 8 karakterlik bir parola büyük ve küçük harflerin bir kombinasyonunu içermelidir.")]
        public string Password { get; set; }
    }

    public class UpdateUser
    {
        [StringLength(50, ErrorMessage = "50 karakterden fazla giriş yapılamaz.")]
        [Required(ErrorMessage = "İsim boş bırakılamaz.")]
        public string FirstName { get; set; }

        [StringLength(50, ErrorMessage = "50 karakterden fazla giriş yapılamaz.")]
        [Required(ErrorMessage = "Soyisim boş bırakılamaz.")]
        public string LastName { get; set; }

        [StringLength(50, ErrorMessage = "50 karakterden fazla giriş yapılamaz.")]
        [Required(ErrorMessage = "Kullanıcı adı boş bırakılamaz.")]
        public string UserName { get; set; }

        [StringLength(50, ErrorMessage = "50 karakterden fazla giriş yapılamaz.")]
        [Required(ErrorMessage = "Email boş bırakılamaz.")]
        [EmailAddress(ErrorMessage = "Geçerli bir email adresi giriniz.")]
        public string Email { get; set; }

        public DateTime? Age { get; set; }
    }

    public class UpdatePassword
    {
        [StringLength(50, ErrorMessage = "50 karakterden fazla giriş yapılamaz.")]
        [Required(ErrorMessage = "Kullanıcı adı boş bırakılamaz.")]
        public string UserName { get; set; }

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Lütfen şifre giriniz.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])[a-zA-Z\d]{8,}$", ErrorMessage = "En az 8 karakterlik bir parola büyük ve küçük harflerin bir kombinasyonunu içermelidir.")]
        public string Password { get; set; }
    }

    public class UpdatePasswordApiResponse
    {
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
        public UpdatePassword Data { get; set; }
    }

    public class UserList : User
    {
        public long Id { get; set; }
    }

    public class LoginUserViewModel : User
    {
        public long Id { get; set; }
    }

	public class UpdateProfilePhoto
	{
		public string UserName { get; set; }
		public IFormFile ProfileImage { get; set; }
	}

	public class ProfilePhotoApiResponse
    {
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
        public UpdateProfilePhoto Data { get; set; }
    }

    public class UserProfileDataResponse
    {
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
        public User Data { get; set; }
    }

    public class UserApiResponse
    {
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
        public List<UserList> Data { get; set; }
        public int TotalCount { get; set; }
    }

    public class UserUpdateApiResponse
    {
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
        public List<UpdateUser> Data { get; set; }
    }
}
