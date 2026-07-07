using Microsoft.EntityFrameworkCore;
using PracticeTL.Api.Data;
using PracticeTL.Api.Dtos;
using PracticeTL.Api.Models;

namespace PracticeTL.Api.Services;

public class OfficeService : IOfficeService
{
    private readonly AppDbContext _db;
    public OfficeService(AppDbContext db) { _db = db; }

    public async Task<List<Office>> GetAllAsync()
        => await _db.Offices.OrderBy(o => o.SortOrder).ToListAsync();

    public async Task<Office> AddAsync(OfficeInput input)
    {
        Validate(input);
        var has = await _db.Offices.AnyAsync();
        var maxOrder = has ? await _db.Offices.MaxAsync(o => o.SortOrder) : 0;

        var office = new Office
        {
            ImageUrl = input.ImageUrl,
            SortOrder = maxOrder + 1
        };
        _db.Offices.Add(office);
        await _db.SaveChangesAsync();
        return office;
    }

    public async Task<Office?> UpdateAsync(int id, OfficeInput input)
    {
        Validate(input);
        var office = await _db.Offices.FindAsync(id);
        if (office is null) return null;
        office.ImageUrl = input.ImageUrl;
        await _db.SaveChangesAsync();
        return office;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var office = await _db.Offices.FindAsync(id);
        if (office is null) return false;
        _db.Offices.Remove(office);
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task ReorderAsync(List<int> orderedIds)
    {
        var items = await _db.Offices.ToListAsync();
        for (int i = 0; i < orderedIds.Count; i++)
        {
            var o = items.FirstOrDefault(x => x.Id == orderedIds[i]);
            if (o != null) o.SortOrder = i + 1;
        }
        await _db.SaveChangesAsync();
    }

    private static void Validate(OfficeInput input)
    {
        if (string.IsNullOrWhiteSpace(input.ImageUrl))
            throw new ArgumentException("Ссылка на фото не может быть пустой");
    }
}
