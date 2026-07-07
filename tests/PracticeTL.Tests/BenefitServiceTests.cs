using PracticeTL.Api.Dtos;
using PracticeTL.Api.Services;
using Xunit;

namespace PracticeTL.Tests;

public class BenefitServiceTests
{
    [Fact]
    public async Task AddAsync_AssignsNextSortOrder()
    {
        using var db = TestDb.Create();
        var service = new BenefitService(db);

        var first = await service.AddAsync(new BenefitInput { Title = "Обед", Text = "Кормим" });
        var second = await service.AddAsync(new BenefitInput { Title = "Спорт", Text = "Абонемент" });

        Assert.Equal(1, first.SortOrder);
        Assert.Equal(2, second.SortOrder);
        Assert.Equal(2, (await service.GetAllAsync()).Count);
    }

    [Fact]
    public async Task AddAsync_EmptyTitle_Throws()
    {
        using var db = TestDb.Create();
        var service = new BenefitService(db);

        await Assert.ThrowsAsync<ArgumentException>(() =>
            service.AddAsync(new BenefitInput { Title = "", Text = "x" }));
    }

    [Fact]
    public async Task UpdateAsync_ChangesFields()
    {
        using var db = TestDb.Create();
        var service = new BenefitService(db);
        var b = await service.AddAsync(new BenefitInput { Title = "Старое", Text = "текст" });

        var updated = await service.UpdateAsync(b.Id, new BenefitInput { Title = "Новое", Text = "текст2" });

        Assert.NotNull(updated);
        Assert.Equal("Новое", updated!.Title);
        Assert.Equal("текст2", updated.Text);
    }

    [Fact]
    public async Task UpdateAsync_MissingId_ReturnsNull()
    {
        using var db = TestDb.Create();
        var service = new BenefitService(db);

        var result = await service.UpdateAsync(999, new BenefitInput { Title = "x", Text = "y" });

        Assert.Null(result);
    }

    [Fact]
    public async Task DeleteAsync_RemovesItem()
    {
        using var db = TestDb.Create();
        var service = new BenefitService(db);
        var b = await service.AddAsync(new BenefitInput { Title = "Удалить", Text = "z" });

        var ok = await service.DeleteAsync(b.Id);

        Assert.True(ok);
        Assert.Empty(await service.GetAllAsync());
    }
}
