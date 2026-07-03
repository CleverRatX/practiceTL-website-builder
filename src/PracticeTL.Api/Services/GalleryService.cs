using Microsoft.EntityFrameworkCore;
using PracticeTL.Api.Data;
using PracticeTL.Api.Dtos;
using PracticeTL.Api.Models;

namespace PracticeTL.Api.Services;

public class GalleryService : IGalleryService
{
    private readonly AppDbContext _db;
    public GalleryService(AppDbContext db) { _db = db; }

    public async Task<List<GalleryItem>> GetAllAsync()
        => await _db.GalleryItems.OrderBy(g => g.SortOrder).ToListAsync();

    public async Task<GalleryItem> AddAsync(GalleryItemInput input)
    {
        Validate(input);
        var has = await _db.GalleryItems.AnyAsync();
        var maxOrder = has ? await _db.GalleryItems.MaxAsync(g => g.SortOrder) : 0;

        var item = new GalleryItem
        {
            ImageUrl = input.ImageUrl,
            Caption = input.Caption,
            SortOrder = maxOrder + 1
        };
        _db.GalleryItems.Add(item);
        await _db.SaveChangesAsync();
        return item;
    }

    public async Task<GalleryItem?> UpdateAsync(int id, GalleryItemInput input)
    {
        Validate(input);
        var item = await _db.GalleryItems.FindAsync(id);
        if (item is null) return null;
        item.ImageUrl = input.ImageUrl;
        item.Caption = input.Caption;
        await _db.SaveChangesAsync();
        return item;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var item = await _db.GalleryItems.FindAsync(id);
        if (item is null) return false;
        _db.GalleryItems.Remove(item);
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task ReorderAsync(List<int> orderedIds)
    {
        var items = await _db.GalleryItems.ToListAsync();
        for (int i = 0; i < orderedIds.Count; i++)
        {
            var g = items.FirstOrDefault(x => x.Id == orderedIds[i]);
            if (g != null) g.SortOrder = i + 1;
        }
        await _db.SaveChangesAsync();
    }

    private static void Validate(GalleryItemInput input)
    {
        if (string.IsNullOrWhiteSpace(input.ImageUrl))
            throw new ArgumentException("Ссылка на изображение не может быть пустой");
    }
}
