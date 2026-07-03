using PracticeTL.Api.Dtos;
using PracticeTL.Api.Models;

namespace PracticeTL.Api.Services;

public interface IHeroService
{
    Task<Hero> GetHeroAsync();
    Task UpdateInfoAsync(HeroInfoInput input);

    Task<HeroStat> AddStatAsync(HeroStatInput input);
    Task<HeroStat?> UpdateStatAsync(int id, HeroStatInput input);
    Task<bool> DeleteStatAsync(int id);
    Task ReorderStatsAsync(List<int> orderedIds);
}
