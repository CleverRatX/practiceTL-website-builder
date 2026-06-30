using Microsoft.EntityFrameworkCore;
using PracticeTL.Api.Data;
using PracticeTL.Api.Dtos;
using PracticeTL.Api.Models;

namespace PracticeTL.Api.Services;

public class VacancyService : IVacancyService
{
    private readonly AppDbContext _db;
    public VacancyService(AppDbContext db) { _db = db; }

    public async Task<List<Vacancy>> GetAllAsync()
        => await _db.Vacancies.OrderBy(v => v.SortOrder).ToListAsync();

    public async Task<Vacancy> AddAsync(VacancyInput input)
    {
        Validate(input);
        var has = await _db.Vacancies.AnyAsync();
        var maxOrder = has ? await _db.Vacancies.MaxAsync(v => v.SortOrder) : 0;

        var vacancy = new Vacancy
        {
            Title = input.Title,
            Address = input.Address,
            Url = input.Url,
            SortOrder = maxOrder + 1
        };
        _db.Vacancies.Add(vacancy);
        await _db.SaveChangesAsync();
        return vacancy;
    }

    public async Task<Vacancy?> UpdateAsync(int id, VacancyInput input)
    {
        Validate(input);
        var vacancy = await _db.Vacancies.FindAsync(id);
        if (vacancy is null) return null;
        vacancy.Title = input.Title;
        vacancy.Address = input.Address;
        vacancy.Url = input.Url;
        await _db.SaveChangesAsync();
        return vacancy;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var vacancy = await _db.Vacancies.FindAsync(id);
        if (vacancy is null) return false;
        _db.Vacancies.Remove(vacancy);
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task ReorderAsync(List<int> orderedIds)
    {
        var items = await _db.Vacancies.ToListAsync();
        for (int i = 0; i < orderedIds.Count; i++)
        {
            var v = items.FirstOrDefault(x => x.Id == orderedIds[i]);
            if (v != null) v.SortOrder = i + 1;
        }
        await _db.SaveChangesAsync();
    }

    private static void Validate(VacancyInput input)
    {
        if (string.IsNullOrWhiteSpace(input.Title))
            throw new ArgumentException("Заголовок вакансии не может быть пустым");
    }
}
