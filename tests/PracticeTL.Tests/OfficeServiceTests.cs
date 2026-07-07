using PracticeTL.Api.Dtos;
using PracticeTL.Api.Services;
using Xunit;

namespace PracticeTL.Tests;

public class OfficeServiceTests
{
    [Fact]
    public async Task AddAsync_AssignsNextSortOrder()
    {
        using var db = TestDb.Create();
        var service = new OfficeService(db);

        var first = await service.AddAsync(new OfficeInput { ImageUrl = "/media/main/offices/1.jpg" });
        var second = await service.AddAsync(new OfficeInput { ImageUrl = "/media/main/offices/2.jpg" });

        Assert.Equal(1, first.SortOrder);
        Assert.Equal(2, second.SortOrder);
        Assert.Equal(2, (await service.GetAllAsync()).Count);
    }

    [Fact]
    public async Task AddAsync_EmptyImageUrl_Throws()
    {
        using var db = TestDb.Create();
        var service = new OfficeService(db);

        await Assert.ThrowsAsync<ArgumentException>(() =>
            service.AddAsync(new OfficeInput { ImageUrl = "" }));
    }

    [Fact]
    public async Task UpdateAsync_ChangesImageUrl()
    {
        using var db = TestDb.Create();
        var service = new OfficeService(db);
        var o = await service.AddAsync(new OfficeInput { ImageUrl = "/old.jpg" });

        var updated = await service.UpdateAsync(o.Id, new OfficeInput { ImageUrl = "/new.jpg" });

        Assert.NotNull(updated);
        Assert.Equal("/new.jpg", updated!.ImageUrl);
    }

    [Fact]
    public async Task DeleteAsync_RemovesItem()
    {
        using var db = TestDb.Create();
        var service = new OfficeService(db);
        var o = await service.AddAsync(new OfficeInput { ImageUrl = "/x.jpg" });

        var ok = await service.DeleteAsync(o.Id);

        Assert.True(ok);
        Assert.Empty(await service.GetAllAsync());
    }

    [Fact]
    public async Task ReorderAsync_ChangesOrder()
    {
        using var db = TestDb.Create();
        var service = new OfficeService(db);
        var a = await service.AddAsync(new OfficeInput { ImageUrl = "/a.jpg" });
        var b = await service.AddAsync(new OfficeInput { ImageUrl = "/b.jpg" });

        await service.ReorderAsync(new List<int> { b.Id, a.Id });

        var all = await service.GetAllAsync();
        Assert.Equal(b.Id, all[0].Id);
        Assert.Equal(a.Id, all[1].Id);
    }
}
