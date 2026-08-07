using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using IdentityService.Api.Data;
using IdentityService.Api.Models;
using IdentityService.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppIdentityDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddDataProtection();

builder.Services.AddIdentityCore<ApplicationUser>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;

    // Login'de lockoutOnFailure: true kullandığın için bunlar önemli
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.AllowedForNewUsers = true;

    options.User.RequireUniqueEmail = true;
})
.AddRoles<IdentityRole<Guid>>()          // ileride rol bazlı yetkilendirme için (Admin, Seller vs.)
.AddSignInManager()                       // 🔑 asıl eksik olan buydu — SignInManager artık DI'a kayıtlı
.AddEntityFrameworkStores<AppIdentityDbContext>()
.AddDefaultTokenProviders();

builder.Services.AddScoped<TokenService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Şu an hiçbir endpoint [Authorize] değil, o yüzden şart değil ama
// ileride "me", "change-password" gibi korumalı endpoint eklersen
// UseAuthorization'dan ÖNCE mutlaka UseAuthentication olmalı.
// app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();