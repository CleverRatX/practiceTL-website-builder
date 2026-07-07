using Microsoft.EntityFrameworkCore;
using PracticeTL.Api.Data;
using PracticeTL.Api.Dtos;
using PracticeTL.Api.Models;

namespace PracticeTL.Api.Services;

public class BenefitService : IBenefitService
{
    private readonly AppDbContext _db;
    public BenefitService(AppDbContext db) { _db = db; }

    public async Task<List<Benefit>> GetAllAsync()
        => await _db.Benefits.OrderBy(b => b.SortOrder).ToListAsync();

    public async Task<Benefit> AddAsync(BenefitInput input)
    {
        Validate(input);
        var has = await _db.Benefits.AnyAsync();
        var maxOrder = has ? await _db.Benefits.MaxAsync(b => b.SortOrder) : 0;

        var benefit = new Benefit
        {
            Title = input.Title,
            Text = input.Text,
            SortOrder = maxOrder + 1
        };
        _db.Benefits.Add(benefit);
        await _db.SaveChangesAsync();
        return benefit;
    }

    public async Task<Benefit?> UpdateAsync(int id, BenefitInput input)
    {
        Validate(input);
        var benefit = await _db.Benefits.FindAsync(id);
        if (benefit is null) return null;
        benefit.Title = input.Title;
        benefit.Text = input.Text;
        await _db.SaveChangesAsync();
        return benefit;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var benefit = await _db.Benefits.FindAsync(id);
        if (benefit is null) return false;
        _db.Benefits.Remove(benefit);
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task ReorderAsync(List<int> orderedIds)
    {
        var items = await _db.Benefits.ToListAsync();
        for (int i = 0; i < orderedIds.Count; i++)
        {
            var b = items.FirstOrDefault(x => x.Id == orderedIds[i]);
            if (b != null) b.SortOrder = i + 1;
        }
        await _db.SaveChangesAsync();
    }

    private static void Validate(BenefitInput input)
    {
        if (string.IsNullOrWhiteSpace(input.Title))
            throw new ArgumentException("Заголовок плюшки не может быть пустым");
    }
}
