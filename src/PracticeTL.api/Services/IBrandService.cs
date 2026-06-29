using PracticeTL.Api.Dtos;
using PracticeTL.Api.Models;

namespace PracticeTL.Api.Services;

public interface IBrandService
{
    Task<List<Brand>> GetAllAsync();
    Task<Brand> AddAsync(BrandInput input);
    Task<Brand?> UpdateAsync(int id, BrandInput input);
    Task<bool> DeleteAsync(int id);
    Task ReorderAsync(List<int> orderedIds);
}
