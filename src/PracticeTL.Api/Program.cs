using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using PracticeTL.Api.Data;
using PracticeTL.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/login.html";
        options.AccessDeniedPath = "/login.html";
    });
builder.Services.AddAuthorization();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IHeroService, HeroService>();
builder.Services.AddScoped<ITeamService, TeamService>();
builder.Services.AddScoped<IPlatformService, PlatformService>();
builder.Services.AddScoped<IBrandService, BrandService>();
builder.Services.AddScoped<IDirectionService, DirectionService>();
builder.Services.AddScoped<IVacancyService, VacancyService>();
builder.Services.AddScoped<IGalleryService, GalleryService>();
builder.Services.AddScoped<IBlockSettingService, BlockSettingService>();
builder.Services.AddScoped<IBenefitService, BenefitService>();
builder.Services.AddScoped<IOfficeService, OfficeService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
    HeroSeeder.Seed(db);
    TeamSeeder.Seed(db);
    PlatformSeeder.Seed(db);
    BrandSeeder.Seed(db);
    DirectionSeeder.Seed(db);
    VacancySeeder.Seed(db);
    GallerySeeder.Seed(db);
    BlockSettingSeeder.Seed(db);
    BenefitSeeder.Seed(db);
    OfficeSeeder.Seed(db);
}

app.UseSwagger();
app.UseSwaggerUI();

app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
