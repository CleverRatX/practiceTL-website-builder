using Microsoft.EntityFrameworkCore;
using PracticeTL.Api.Data;
using PracticeTL.Api.Dtos;
using PracticeTL.Api.Models;

namespace PracticeTL.Api.Services;

public class PlatformService : IPlatformService
{
    private readonly AppDbContext _db;
    public PlatformService(AppDbContext db) { _db = db; }

    public async Task<List<PlatformItem>> GetAllAsync()
        => await _db.PlatformItems.OrderBy(p => p.SortOrder).ToListAsync();

    public async Task<PlatformItem> AddAsync(PlatformItemInput input)
    {
        Validate(input);
        var has = await _db.PlatformItems.AnyAsync();
        var maxOrder = has ? await _db.PlatformItems.MaxAsync(p => p.SortOrder) : 0;

        var item = new PlatformItem
        {
            Year = input.Year,
            Name = input.Name,
            Description = input.Description,
            Variant = 1,
            Icon = "",
            SortOrder = maxOrder + 1
        };
        _db.PlatformItems.Add(item);
        await _db.SaveChangesAsync();
        return item;
    }

    public async Task<PlatformItem?> UpdateAsync(int id, PlatformItemInput input)
    {
        Validate(input);
        var item = await _db.PlatformItems.FindAsync(id);
        if (item is null) return null;

        item.Year = input.Year;
        item.Name = input.Name;
        item.Description = input.Description;
        await _db.SaveChangesAsync();
        return item;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var item = await _db.PlatformItems.FindAsync(id);
        if (item is null) return false;
        _db.PlatformItems.Remove(item);
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task ReorderAsync(List<int> orderedIds)
    {
        var items = await _db.PlatformItems.ToListAsync();
        for (int i = 0; i < orderedIds.Count; i++)
        {
            var item = items.FirstOrDefault(x => x.Id == orderedIds[i]);
            if (item != null) item.SortOrder = i + 1;
        }
        await _db.SaveChangesAsync();
    }

    private static void Validate(PlatformItemInput input)
    {
        if (string.IsNullOrWhiteSpace(input.Name))
            throw new ArgumentException("Название не может быть пустым");
        if (string.IsNullOrWhiteSpace(input.Year))
            throw new ArgumentException("Год не может быть пустым");
    }
}
