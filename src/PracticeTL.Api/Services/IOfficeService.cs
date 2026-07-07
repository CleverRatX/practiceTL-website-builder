using PracticeTL.Api.Dtos;
using PracticeTL.Api.Models;

namespace PracticeTL.Api.Services;

public interface IOfficeService
{
    Task<List<Office>> GetAllAsync();
    Task<Office> AddAsync(OfficeInput input);
    Task<Office?> UpdateAsync(int id, OfficeInput input);
    Task<bool> DeleteAsync(int id);
    Task ReorderAsync(List<int> orderedIds);
}
