using Microsoft.EntityFrameworkCore;
using PracticeTL.Api.Data;
using PracticeTL.Api.Dtos;
using PracticeTL.Api.Models;

namespace PracticeTL.Api.Services;

public class HeroService : IHeroService
{
    private readonly AppDbContext _db;

    public HeroService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<Hero> GetHeroAsync()
    {
        var hero = await _db.Heroes
            .Include(h => h.Stats.OrderBy(s => s.SortOrder))
            .FirstAsync();
        return hero;
    }

    public async Task UpdateInfoAsync(HeroInfoInput input)
    {
        var hero = await _db.Heroes.FirstAsync();
        hero.Title = input.Title;
        hero.Subtitle = input.Subtitle;
        await _db.SaveChangesAsync();
    }

    public async Task<HeroStat> AddStatAsync(HeroStatInput input)
    {
        Validate(input);

        var hero = await _db.Heroes.Include(h => h.Stats).FirstAsync();
        var maxOrder = hero.Stats.Count > 0 ? hero.Stats.Max(s => s.SortOrder) : 0;

        var stat = new HeroStat
        {
            Type = input.Type,
            Value = input.Value,
            Label = input.Label,
            SortOrder = maxOrder + 1,
            HeroId = hero.Id
        };

        _db.HeroStats.Add(stat);
        await _db.SaveChangesAsync();
        return stat;
    }

    public async Task<HeroStat?> UpdateStatAsync(int id, HeroStatInput input)
    {
        Validate(input);

        var stat = await _db.HeroStats.FindAsync(id);
        if (stat is null)
            return null;

        stat.Type = input.Type;
        stat.Value = input.Value;
        stat.Label = input.Label;
        await _db.SaveChangesAsync();
        return stat;
    }

    public async Task<bool> DeleteStatAsync(int id)
    {
        var stat = await _db.HeroStats.FindAsync(id);
        if (stat is null)
            return false;

        _db.HeroStats.Remove(stat);
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task ReorderStatsAsync(List<int> orderedIds)
    {
        var stats = await _db.HeroStats.ToListAsync();
        for (int i = 0; i < orderedIds.Count; i++)
        {
            var stat = stats.FirstOrDefault(s => s.Id == orderedIds[i]);
            if (stat != null)
                stat.SortOrder = i + 1;
        }
        await _db.SaveChangesAsync();
    }

    private static void Validate(HeroStatInput input)
    {
        if (string.IsNullOrWhiteSpace(input.Value))
            throw new ArgumentException("Поле value не может быть пустым");

        if (input.Type != "text" && input.Type != "img")
            throw new ArgumentException("Поле type должно быть 'text' или 'img'");
    }
}
