using MarketPlaceSellerApp.AuthenticationHandler;
using MarketPlaceSellerApp.FileNameGuid;
using MarketPlaceSellerApp.Helpers;
using MarketPlaceSellerApp.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();


builder.Services.AddSwaggerGen(c =>
{
	c.AddSecurityDefinition("basic", new OpenApiSecurityScheme
	{
		In = ParameterLocation.Header,
		Description = "Basic authentication header using the basic scheme.",
		Name = "Authorization",
		Type = SecuritySchemeType.Http,
		Scheme = "basic"
	});

	c.AddSecurityRequirement(new OpenApiSecurityRequirement
	{
		{
			new OpenApiSecurityScheme
			{
				Reference = new OpenApiReference
				{
					Type = ReferenceType.SecurityScheme,
					Id = "basic"
				}
			},
			new string[] { }
		}
	});
});

builder.Services.AddTransient<AuthHelpers>();
builder.Services.AddSingleton<GuidOperation>();

builder.Services.AddSingleton<ISystemClock, SystemClock>();

builder.Services.AddAuthentication("BasicAuthentication")
	.AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>("BasicAuthentication", null);

builder.Services.AddDbContext<HepsiburadaSellerInformationContext>(context =>
{
	context.UseSqlServer(@"Data Source=PRASOFT\SQLEXPRESS;Initial Catalog=Hepsiburada-Seller-Information;Persist Security Info=True;Trusted_Connection=True;TrustServerCertificate=Yes;");
});

builder.Services.AddAuthorization();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI(c =>
	{
		c.SwaggerEndpoint("/swagger/v1/swagger.json","MobilApp.Api V1");
		c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None);
	});
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
