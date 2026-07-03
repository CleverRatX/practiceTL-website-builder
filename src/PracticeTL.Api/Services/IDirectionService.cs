using PracticeTL.Api.Dtos;
using PracticeTL.Api.Models;

namespace PracticeTL.Api.Services;

public interface IDirectionService
{
    Task<List<Direction>> GetAllAsync();
    Task<Direction> AddAsync(DirectionInput input);
    Task<Direction?> UpdateAsync(int id, DirectionInput input);
    Task<bool> DeleteAsync(int id);
    Task ReorderAsync(List<int> orderedIds);
}
