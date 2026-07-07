using PracticeTL.Api.Data;
using PracticeTL.Api.Models;
using PracticeTL.Api.Services;
using Xunit;

namespace PracticeTL.Tests;

public class BlockSettingServiceTests
{
    private static async Task SeedAsync(AppDbContext db)
    {
        db.BlockSettings.AddRange(
            new BlockSetting { Key = "gallery", Title = "Фотогалерея", Visible = true, SortOrder = 3 },
            new BlockSetting { Key = "team", Title = "Команда", Visible = true, SortOrder = 1 },
            new BlockSetting { Key = "platform", Title = "Платформа", Visible = true, SortOrder = 2 }
        );
        await db.SaveChangesAsync();
    }

    [Fact]
    public async Task GetAllAsync_ReturnsOrderedBySortOrder()
    {
        using var db = TestDb.Create();
        await SeedAsync(db);
        var service = new BlockSettingService(db);

        var all = await service.GetAllAsync();

        Assert.Equal(3, all.Count);
        Assert.Equal("team", all[0].Key);
        Assert.Equal("platform", all[1].Key);
        Assert.Equal("gallery", all[2].Key);
    }

    [Fact]
    public async Task SetVisibleAsync_TogglesVisibility()
    {
        using var db = TestDb.Create();
        await SeedAsync(db);
        var service = new BlockSettingService(db);

        var updated = await service.SetVisibleAsync("team", false);

        Assert.NotNull(updated);
        Assert.False(updated!.Visible);

        foreach (var b in await service.GetAllAsync())
            if (b.Key == "team")
                Assert.False(b.Visible);
    }

    [Fact]
    public async Task SetVisibleAsync_UnknownKey_ReturnsNull()
    {
        using var db = TestDb.Create();
        await SeedAsync(db);
        var service = new BlockSettingService(db);

        var result = await service.SetVisibleAsync("nope", false);

        Assert.Null(result);
    }
}
