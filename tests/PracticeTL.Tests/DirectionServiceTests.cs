using PracticeTL.Api.Dtos;
using PracticeTL.Api.Services;
using Xunit;

namespace PracticeTL.Tests;

public class DirectionServiceTests
{
    [Fact]
    public async Task AddAsync_AddsDirection_WithSortOrderAndContent()
    {
        using var db = TestDb.Create();
        var service = new DirectionService(db);

        var first = await service.AddAsync(new DirectionInput { Name = "Backend", Badges = "C#, SQL", Content = "<p>описание</p>" });
        var second = await service.AddAsync(new DirectionInput { Name = "Frontend", Badges = "React", Content = "" });

        Assert.Equal(1, first.SortOrder);
        Assert.Equal(2, second.SortOrder);
        Assert.Equal("C#, SQL", first.Badges);
        Assert.Equal("<p>описание</p>", first.Content);
    }

    [Fact]
    public async Task AddAsync_EmptyName_Throws()
    {
        using var db = TestDb.Create();
        var service = new DirectionService(db);

        await Assert.ThrowsAsync<ArgumentException>(() =>
            service.AddAsync(new DirectionInput { Name = "", Badges = "C#", Content = "x" }));
    }

    [Fact]
    public async Task UpdateAsync_ChangesNameBadgesContent()
    {
        using var db = TestDb.Create();
        var service = new DirectionService(db);
        var d = await service.AddAsync(new DirectionInput { Name = "Старое", Badges = "x", Content = "старое" });

        var updated = await service.UpdateAsync(d.Id, new DirectionInput { Name = "Новое", Badges = "y", Content = "новое" });

        Assert.NotNull(updated);
        Assert.Equal("Новое", updated!.Name);
        Assert.Equal("y", updated.Badges);
        Assert.Equal("новое", updated.Content);
    }

    [Fact]
    public async Task UpdateAsync_MissingId_ReturnsNull()
    {
        using var db = TestDb.Create();
        var service = new DirectionService(db);

        Assert.Null(await service.UpdateAsync(999, new DirectionInput { Name = "x", Badges = "y", Content = "z" }));
    }

    [Fact]
    public async Task DeleteAsync_RemovesDirection()
    {
        using var db = TestDb.Create();
        var service = new DirectionService(db);
        var d = await service.AddAsync(new DirectionInput { Name = "x", Badges = "y", Content = "z" });

        Assert.True(await service.DeleteAsync(d.Id));
        Assert.Empty(await service.GetAllAsync());
    }

    [Fact]
    public async Task DeleteAsync_MissingId_ReturnsFalse()
    {
        using var db = TestDb.Create();
        var service = new DirectionService(db);

        Assert.False(await service.DeleteAsync(999));
    }

    [Fact]
    public async Task ReorderAsync_ChangesOrder()
    {
        using var db = TestDb.Create();
        var service = new DirectionService(db);
        var a = await service.AddAsync(new DirectionInput { Name = "A", Badges = "x", Content = "" });
        var b = await service.AddAsync(new DirectionInput { Name = "B", Badges = "x", Content = "" });

        await service.ReorderAsync(new List<int> { b.Id, a.Id });

        var all = await service.GetAllAsync();
        Assert.Equal("B", all[0].Name);
        Assert.Equal("A", all[1].Name);
    }
}
