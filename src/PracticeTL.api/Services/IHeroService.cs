using PracticeTL.Api.Dtos;
using PracticeTL.Api.Models;

namespace PracticeTL.Api.Services;

public interface IHeroService
{
    Task<List<HeroItem>> GetAllAsync();
    Task<HeroItem?> GetByIdAsync(int id);
    Task<HeroItem> CreateAsync(HeroItemInput input);
    Task<HeroItem?> UpdateAsync(int id, HeroItemInput input);
    Task<bool> DeleteAsync(int id);
    Task ReorderAsync(List<int> orderedIds); 
}
