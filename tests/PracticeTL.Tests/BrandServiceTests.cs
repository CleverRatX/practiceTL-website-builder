using PracticeTL.Api.Dtos;
using PracticeTL.Api.Services;
using Xunit;

namespace PracticeTL.Tests;

public class BrandServiceTests
{
    [Fact]
    public async Task AddAsync_ДобавляетБренд_СоСледующимПорядком()
    {
        using var db = TestDb.Create();
        var service = new BrandService(db);

        var first = await service.AddAsync(new BrandInput { Name = "Cosmos", Logo = "/media/main/hotels/cosmos.svg" });
        var second = await service.AddAsync(new BrandInput { Name = "Rus", Logo = "/media/main/hotels/rus.svg" });

        Assert.Equal(1, first.SortOrder);
        Assert.Equal(2, second.SortOrder);
        Assert.Equal(2, (await service.GetAllAsync()).Count);
    }

    [Fact]
    public async Task AddAsync_ПустойЛоготип_БросаетОшибку()
    {
        using var db = TestDb.Create();
        var service = new BrandService(db);

        await Assert.ThrowsAsync<ArgumentException>(() =>
            service.AddAsync(new BrandInput { Name = "Cosmos", Logo = "" }));
    }

    [Fact]
    public async Task UpdateAsync_МеняетПоля()
    {
        using var db = TestDb.Create();
        var service = new BrandService(db);
        var b = await service.AddAsync(new BrandInput { Name = "Старое", Logo = "/old.svg" });

        var updated = await service.UpdateAsync(b.Id, new BrandInput { Name = "Новое", Logo = "/new.svg" });

        Assert.NotNull(updated);
        Assert.Equal("Новое", updated!.Name);
        Assert.Equal("/new.svg", updated.Logo);
    }

    [Fact]
    public async Task UpdateAsync_НесуществующийId_ВозвращаетNull()
    {
        using var db = TestDb.Create();
        var service = new BrandService(db);

        Assert.Null(await service.UpdateAsync(999, new BrandInput { Name = "x", Logo = "/x.svg" }));
    }

    [Fact]
    public async Task DeleteAsync_УдаляетБренд()
    {
        using var db = TestDb.Create();
        var service = new BrandService(db);
        var b = await service.AddAsync(new BrandInput { Name = "x", Logo = "/x.svg" });

        Assert.True(await service.DeleteAsync(b.Id));
        Assert.Empty(await service.GetAllAsync());
    }

    [Fact]
    public async Task ReorderAsync_ПереставляетПорядок()
    {
        using var db = TestDb.Create();
        var service = new BrandService(db);
        var a = await service.AddAsync(new BrandInput { Name = "A", Logo = "/a.svg" });
        var b = await service.AddAsync(new BrandInput { Name = "B", Logo = "/b.svg" });

        await service.ReorderAsync(new List<int> { b.Id, a.Id });

        var all = await service.GetAllAsync();
        Assert.Equal("B", all[0].Name);
        Assert.Equal("A", all[1].Name);
    }
}
