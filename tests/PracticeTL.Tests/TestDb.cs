using Microsoft.EntityFrameworkCore;
using PracticeTL.Api.Data;
using PracticeTL.Api.Models;

namespace PracticeTL.Tests;

public static class TestDb
{
    public static AppDbContext Create()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }

    public static async Task<Hero> SeedHeroAsync(AppDbContext db)
    {
        var hero = new Hero { Title = "TL:TECH", Subtitle = "Стабильно растем" };
        db.Heroes.Add(hero);
        await db.SaveChangesAsync();
        return hero;
    }
}
