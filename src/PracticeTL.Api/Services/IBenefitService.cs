using PracticeTL.Api.Dtos;
using PracticeTL.Api.Models;

namespace PracticeTL.Api.Services;

public interface IBenefitService
{
    Task<List<Benefit>> GetAllAsync();
    Task<Benefit> AddAsync(BenefitInput input);
    Task<Benefit?> UpdateAsync(int id, BenefitInput input);
    Task<bool> DeleteAsync(int id);
    Task ReorderAsync(List<int> orderedIds);
}
