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

    public async Task<List<HeroItem>> GetAllAsync()
    {
        return await _db.HeroItems
            .OrderBy(item => item.SortOrder)
            .ToListAsync();
    }

    public async Task<HeroItem?> GetByIdAsync(int id)
    {
        return await _db.HeroItems.FindAsync(id);
    }

    public async Task<HeroItem> CreateAsync(HeroItemInput input)
    {
        Validate(input);

        var item = new HeroItem
        {
            Type = input.Type,
            Value = input.Value,
            Label = input.Label,
            SortOrder = input.SortOrder
        };

        _db.HeroItems.Add(item);
        await _db.SaveChangesAsync();
        return item;
    }

    public async Task<HeroItem?> UpdateAsync(int id, HeroItemInput input)
    {
        Validate(input);

        var item = await _db.HeroItems.FindAsync(id);
        if (item is null)
            return null;

        item.Type = input.Type;
        item.Value = input.Value;
        item.Label = input.Label;
        item.SortOrder = input.SortOrder;

        await _db.SaveChangesAsync();
        return item;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var item = await _db.HeroItems.FindAsync(id);
        if (item is null)
            return false;

        _db.HeroItems.Remove(item);
        await _db.SaveChangesAsync();
        return true;
    }

    private static void Validate(HeroItemInput input)
    {
        if (string.IsNullOrWhiteSpace(input.Value))
            throw new ArgumentException("Поле value не может быть пустым");

        if (input.Type != "text" && input.Type != "img")
            throw new ArgumentException("Поле type должно быть 'text' или 'img'");
    }
}
