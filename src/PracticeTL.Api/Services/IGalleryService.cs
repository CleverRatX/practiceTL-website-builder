using PracticeTL.Api.Dtos;
using PracticeTL.Api.Models;

namespace PracticeTL.Api.Services;

public interface IGalleryService
{
    Task<List<GalleryItem>> GetAllAsync();
    Task<GalleryItem> AddAsync(GalleryItemInput input);
    Task<GalleryItem?> UpdateAsync(int id, GalleryItemInput input);
    Task<bool> DeleteAsync(int id);
    Task ReorderAsync(List<int> orderedIds);
}
