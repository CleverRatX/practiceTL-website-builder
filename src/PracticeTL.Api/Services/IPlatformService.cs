using PracticeTL.Api.Dtos;
using PracticeTL.Api.Models;

namespace PracticeTL.Api.Services;

public interface IPlatformService
{
    Task<List<PlatformItem>> GetAllAsync();
    Task<PlatformItem> AddAsync(PlatformItemInput input);
    Task<PlatformItem?> UpdateAsync(int id, PlatformItemInput input);
    Task<bool> DeleteAsync(int id);
    Task ReorderAsync(List<int> orderedIds);
}
