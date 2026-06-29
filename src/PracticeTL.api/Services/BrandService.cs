using Microsoft.EntityFrameworkCore;
using PracticeTL.Api.Data;
using PracticeTL.Api.Dtos;
using PracticeTL.Api.Models;

namespace PracticeTL.Api.Services;

public class BrandService : IBrandService
{
    private readonly AppDbContext _db;
    public BrandService(AppDbContext db) { _db = db; }

    public async Task<List<Brand>> GetAllAsync()
        => await _db.Brands.OrderBy(b => b.SortOrder).ToListAsync();

    public async Task<Brand> AddAsync(BrandInput input)
    {
        Validate(input);
        var has = await _db.Brands.AnyAsync();
        var maxOrder = has ? await _db.Brands.MaxAsync(b => b.SortOrder) : 0;

        var brand = new Brand
        {
            Name = input.Name,
            Logo = input.Logo,
            SortOrder = maxOrder + 1
        };
        _db.Brands.Add(brand);
        await _db.SaveChangesAsync();
        return brand;
    }

    public async Task<Brand?> UpdateAsync(int id, BrandInput input)
    {
        Validate(input);
        var brand = await _db.Brands.FindAsync(id);
        if (brand is null) return null;
        brand.Name = input.Name;
        brand.Logo = input.Logo;
        await _db.SaveChangesAsync();
        return brand;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var brand = await _db.Brands.FindAsync(id);
        if (brand is null) return false;
        _db.Brands.Remove(brand);
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task ReorderAsync(List<int> orderedIds)
    {
        var brands = await _db.Brands.ToListAsync();
        for (int i = 0; i < orderedIds.Count; i++)
        {
            var brand = brands.FirstOrDefault(x => x.Id == orderedIds[i]);
            if (brand != null) brand.SortOrder = i + 1;
        }
        await _db.SaveChangesAsync();
    }

    private static void Validate(BrandInput input)
    {
        if (string.IsNullOrWhiteSpace(input.Logo))
            throw new ArgumentException("Путь к логотипу не может быть пустым");
    }
}
