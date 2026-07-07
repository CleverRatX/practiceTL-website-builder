using PracticeTL.Api.Models;

namespace PracticeTL.Api.Services;

public interface IBlockSettingService
{
    Task<List<BlockSetting>> GetAllAsync();
    Task<BlockSetting?> SetVisibleAsync(string key, bool visible);
}
