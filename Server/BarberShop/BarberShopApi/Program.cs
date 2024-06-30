using BarberShopApi;
using BarberShopBL;
using BarberShopBL.Interfaces;
using BarberShopBL.Services;
using BarberShopDB;
using BarberShopDB.EF.Contexts;
using BarberShopDB.Interfaces;
using BarberShopDB.Services;
using BarberShopEntities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args).UseSerilog();
AppSettings appSettings = builder.Configuration.Get<AppSettings>();

builder.Services.Configure<AppSettings>(builder.Configuration);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IUserDB,UserDB>();
builder.Services.AddScoped<IUserBL, UserBL>();
builder.Services.AddScoped<IAppointmentBL, AppointmentBL>();
builder.Services.AddScoped<IAppointmentDB,AppointmentDB>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddAutoMapper(typeof(MapperManager));
builder.Services.AddDbContext<BarberShopContext>(options =>
{
    options.UseSqlServer(appSettings.ConnectionStrings.BarberShop);
});
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = appSettings.Jwt.Issuer,
            ValidAudience = appSettings.Jwt.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(appSettings.Jwt.SecretKey)),
            ClockSkew = TimeSpan.Zero
        };
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                context.Token = context.Request.Cookies[CookiesKeys.AccessToken];
                return Task.CompletedTask;
            },
        };
    });
    

var app = builder.Build();

// Configure the HTTP request pipeline.
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

