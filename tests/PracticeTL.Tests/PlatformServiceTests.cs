using PracticeTL.Api.Dtos;
using PracticeTL.Api.Models;
using PracticeTL.Api.Services;
using Xunit;

namespace PracticeTL.Tests;

public class PlatformServiceTests
{
    [Fact]
    public async Task AddAsync_ДобавляетПродукт_СоСледующимПорядком()
    {
        using var db = TestDb.Create();
        var service = new PlatformService(db);

        var first = await service.AddAsync(new PlatformItemInput { Year = "2008", Name = "Founding" });
        var second = await service.AddAsync(new PlatformItemInput { Year = "2009", Name = "Extranet" });

        Assert.Equal(1, first.SortOrder);
        Assert.Equal(2, second.SortOrder);
        Assert.Equal(1, first.Variant);
    }

    [Fact]
    public async Task AddAsync_ПустоеНазвание_БросаетОшибку()
    {
        using var db = TestDb.Create();
        var service = new PlatformService(db);

        await Assert.ThrowsAsync<ArgumentException>(() =>
            service.AddAsync(new PlatformItemInput { Year = "2024", Name = "" }));
    }

    [Fact]
    public async Task AddAsync_ПустойГод_БросаетОшибку()
    {
        using var db = TestDb.Create();
        var service = new PlatformService(db);

        await Assert.ThrowsAsync<ArgumentException>(() =>
            service.AddAsync(new PlatformItemInput { Year = "", Name = "Billing" }));
    }

    [Fact]
    public async Task UpdateAsync_МеняетГодИНазвание_НоСохраняетЦветИконкуОписание()
    {
        using var db = TestDb.Create();
        var item = new PlatformItem
        {
            Year = "2009", Name = "Booking Engine", Variant = 2,
            Icon = "<svg>иконка</svg>", Description = "<b>описание</b>", SortOrder = 1
        };
        db.PlatformItems.Add(item);
        await db.SaveChangesAsync();
        var service = new PlatformService(db);

        var updated = await service.UpdateAsync(item.Id, new PlatformItemInput { Year = "2010", Name = "Новое имя" });

        Assert.NotNull(updated);
        Assert.Equal("2010", updated!.Year);
        Assert.Equal("Новое имя", updated.Name);
        Assert.Equal(2, updated.Variant);
        Assert.Equal("<svg>иконка</svg>", updated.Icon);
        Assert.Equal("<b>описание</b>", updated.Description);
    }

    [Fact]
    public async Task DeleteAsync_УдаляетПродукт()
    {
        using var db = TestDb.Create();
        var service = new PlatformService(db);
        var item = await service.AddAsync(new PlatformItemInput { Year = "2024", Name = "GMS" });

        Assert.True(await service.DeleteAsync(item.Id));
        Assert.Empty(await service.GetAllAsync());
    }

    [Fact]
    public async Task ReorderAsync_ПереставляетПорядок()
    {
        using var db = TestDb.Create();
        var service = new PlatformService(db);
        var a = await service.AddAsync(new PlatformItemInput { Year = "2008", Name = "A" });
        var b = await service.AddAsync(new PlatformItemInput { Year = "2009", Name = "B" });

        await service.ReorderAsync(new List<int> { b.Id, a.Id });

        var all = await service.GetAllAsync();
        Assert.Equal("B", all[0].Name);
        Assert.Equal("A", all[1].Name);
    }
}
