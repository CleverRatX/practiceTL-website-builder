using Microsoft.EntityFrameworkCore;
using PracticeTL.Api.Data;
using PracticeTL.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// 1. Включаем контроллеры (наш HeroController).
builder.Services.AddControllers();

// 2. Регистрируем подключение к PostgreSQL.
//    Строку подключения берём из appsettings.json (ключ "DefaultConnection").
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// 3. Регистрируем наш сервис. Теперь, когда контроллеру нужен IHeroService,
//    ASP.NET сам создаст HeroService и подставит его (это и есть "внедрение зависимостей").
builder.Services.AddScoped<IHeroService, HeroService>();

var app = builder.Build();

// 4. При старте: применяем миграции (создаём таблицы) и засеваем данные.
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();   // создаёт базу/таблицы по миграциям, если их ещё нет
    HeroSeeder.Seed(db);     // кладёт 8 стартовых элементов, если таблица пуста
}

// 5. Раздаём статические файлы из wwwroot (CSS, картинки, scripts, admin.html).
//    UseDefaultFiles НЕ нужен: главную страницу "/" теперь рендерит HomeController
//    (он подставляет данные из базы в HTML до отправки браузеру).
app.UseStaticFiles();

// 6. Подключаем маршруты контроллеров: "/" и "/index.html" (HomeController)
//    и "/api/hero" (HeroController, его использует админка).
app.MapControllers();

app.Run();
