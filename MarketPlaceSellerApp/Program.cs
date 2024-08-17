//using MarketPlaceSellerApp.AuthenticationHandler;
using MarketPlaceSellerApp.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
	//	c.AddSecurityDefinition("Basic", new OpenApiSecurityScheme
	//	{
	//		In = ParameterLocation.Header,
	//		Description = "Basic Authorization header using the Basic scheme.",
	//		Name = "Authorization",
	//		Type = SecuritySchemeType.Http,
	//		Scheme = "Basic"
	//	});
	//	c.AddSecurityRequirement(new OpenApiSecurityRequirement {
	//		{
	//			new OpenApiSecurityScheme {
	//				Reference = new OpenApiReference {
	//					Type = ReferenceType.SecurityScheme,
	//					Id = "Basic"
	//				}
	//			},
	//			new string[] {}
	//		}
	//	});
});

builder.Services.AddDbContext<HepsiburadaSellerInformationContext>(context =>
{
	context.UseSqlServer(@"Data Source=DESKTOP-PJ1E5QV;Initial Catalog=Hepsiburada-Seller-Information;Persist Security Info=True;Trusted_Connection=True;TrustServerCertificate=Yes;");
});

//builder.Services.AddAuthentication("BasicAuthentication")
	//.AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>("BasicAuthentication", null);

builder.Services.AddAuthorization();


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
