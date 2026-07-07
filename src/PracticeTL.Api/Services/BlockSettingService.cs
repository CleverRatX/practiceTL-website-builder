using Microsoft.EntityFrameworkCore;
using PracticeTL.Api.Data;
using PracticeTL.Api.Models;

namespace PracticeTL.Api.Services;

public class BlockSettingService : IBlockSettingService
{
    private readonly AppDbContext _db;

    public BlockSettingService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<BlockSetting>> GetAllAsync()
    {
        return await _db.BlockSettings
            .OrderBy(b => b.SortOrder)
            .ToListAsync();
    }

    public async Task<BlockSetting?> SetVisibleAsync(string key, bool visible)
    {
        var block = await _db.BlockSettings.FirstOrDefaultAsync(b => b.Key == key);
        if (block is null)
            return null;

        block.Visible = visible;
        await _db.SaveChangesAsync();
        return block;
    }
}
