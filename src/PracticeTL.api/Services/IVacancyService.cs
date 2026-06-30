using PracticeTL.Api.Dtos;
using PracticeTL.Api.Models;

namespace PracticeTL.Api.Services;

public interface IVacancyService
{
    Task<List<Vacancy>> GetAllAsync();
    Task<Vacancy> AddAsync(VacancyInput input);
    Task<Vacancy?> UpdateAsync(int id, VacancyInput input);
    Task<bool> DeleteAsync(int id);
    Task ReorderAsync(List<int> orderedIds);
}
