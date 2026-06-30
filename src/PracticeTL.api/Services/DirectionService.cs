using Microsoft.EntityFrameworkCore;
using PracticeTL.Api.Data;
using PracticeTL.Api.Dtos;
using PracticeTL.Api.Models;

namespace PracticeTL.Api.Services;

public class DirectionService : IDirectionService
{
    private readonly AppDbContext _db;
    public DirectionService(AppDbContext db) { _db = db; }

    public async Task<List<Direction>> GetAllAsync()
        => await _db.Directions.OrderBy(d => d.SortOrder).ToListAsync();

    public async Task<Direction> AddAsync(DirectionInput input)
    {
        Validate(input);
        var has = await _db.Directions.AnyAsync();
        var maxOrder = has ? await _db.Directions.MaxAsync(d => d.SortOrder) : 0;

        var direction = new Direction
        {
            Name = input.Name,
            Badges = input.Badges,
            Content = "",
            SortOrder = maxOrder + 1
        };
        _db.Directions.Add(direction);
        await _db.SaveChangesAsync();
        return direction;
    }

    public async Task<Direction?> UpdateAsync(int id, DirectionInput input)
    {
        Validate(input);
        var direction = await _db.Directions.FindAsync(id);
        if (direction is null) return null;

        direction.Name = input.Name;
        direction.Badges = input.Badges;
        await _db.SaveChangesAsync();
        return direction;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var direction = await _db.Directions.FindAsync(id);
        if (direction is null) return false;
        _db.Directions.Remove(direction);
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task ReorderAsync(List<int> orderedIds)
    {
        var items = await _db.Directions.ToListAsync();
        for (int i = 0; i < orderedIds.Count; i++)
        {
            var d = items.FirstOrDefault(x => x.Id == orderedIds[i]);
            if (d != null) d.SortOrder = i + 1;
        }
        await _db.SaveChangesAsync();
    }

    private static void Validate(DirectionInput input)
    {
        if (string.IsNullOrWhiteSpace(input.Name))
            throw new ArgumentException("Название направления не может быть пустым");
    }
}
