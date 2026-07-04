using PracticeTL.Api.Dtos;
using PracticeTL.Api.Services;
using Xunit;

namespace PracticeTL.Tests;

public class HeroServiceTests
{
    [Fact]
    public async Task UpdateInfoAsync_ChangesTitleAndSubtitle()
    {
        using var db = TestDb.Create();
        await TestDb.SeedHeroAsync(db);
        var service = new HeroService(db);

        await service.UpdateInfoAsync(new HeroInfoInput { Title = "Новый", Subtitle = "Текст" });

        var hero = await service.GetHeroAsync();
        Assert.Equal("Новый", hero.Title);
        Assert.Equal("Текст", hero.Subtitle);
    }

    [Fact]
    public async Task AddStatAsync_AssignsNextSortOrder()
    {
        using var db = TestDb.Create();
        await TestDb.SeedHeroAsync(db);
        var service = new HeroService(db);

        var first = await service.AddStatAsync(new HeroStatInput { Type = "text", Value = "300+", Label = "сотрудников" });
        var second = await service.AddStatAsync(new HeroStatInput { Type = "text", Value = "15", Label = "лет" });

        Assert.True(first.Id > 0);
        Assert.Equal(1, first.SortOrder);
        Assert.Equal(2, second.SortOrder);
        Assert.Equal(2, (await service.GetHeroAsync()).Stats.Count);
    }

    [Fact]
    public async Task AddStatAsync_EmptyValue_Throws()
    {
        using var db = TestDb.Create();
        await TestDb.SeedHeroAsync(db);
        var service = new HeroService(db);

        await Assert.ThrowsAsync<ArgumentException>(() =>
            service.AddStatAsync(new HeroStatInput { Type = "text", Value = "", Label = "x" }));
    }

    [Fact]
    public async Task AddStatAsync_InvalidType_Throws()
    {
        using var db = TestDb.Create();
        await TestDb.SeedHeroAsync(db);
        var service = new HeroService(db);

        await Assert.ThrowsAsync<ArgumentException>(() =>
            service.AddStatAsync(new HeroStatInput { Type = "video", Value = "x", Label = "y" }));
    }

    [Fact]
    public async Task UpdateStatAsync_ChangesFields()
    {
        using var db = TestDb.Create();
        await TestDb.SeedHeroAsync(db);
        var service = new HeroService(db);
        var stat = await service.AddStatAsync(new HeroStatInput { Type = "text", Value = "старое", Label = "x" });

        var updated = await service.UpdateStatAsync(stat.Id, new HeroStatInput { Type = "img", Value = "/logo.svg", Label = "y" });

        Assert.NotNull(updated);
        Assert.Equal("img", updated!.Type);
        Assert.Equal("/logo.svg", updated.Value);
        Assert.Equal("y", updated.Label);
    }

    [Fact]
    public async Task UpdateStatAsync_MissingId_ReturnsNull()
    {
        using var db = TestDb.Create();
        await TestDb.SeedHeroAsync(db);
        var service = new HeroService(db);

        var updated = await service.UpdateStatAsync(999, new HeroStatInput { Type = "text", Value = "x", Label = "y" });

        Assert.Null(updated);
    }

    [Fact]
    public async Task DeleteStatAsync_RemovesStat()
    {
        using var db = TestDb.Create();
        await TestDb.SeedHeroAsync(db);
        var service = new HeroService(db);
        var stat = await service.AddStatAsync(new HeroStatInput { Type = "text", Value = "x", Label = "y" });

        var deleted = await service.DeleteStatAsync(stat.Id);

        Assert.True(deleted);
        Assert.Empty((await service.GetHeroAsync()).Stats);
    }

    [Fact]
    public async Task DeleteStatAsync_MissingId_ReturnsFalse()
    {
        using var db = TestDb.Create();
        await TestDb.SeedHeroAsync(db);
        var service = new HeroService(db);

        Assert.False(await service.DeleteStatAsync(999));
    }

    [Fact]
    public async Task ReorderStatsAsync_ChangesOrderByIdList()
    {
        using var db = TestDb.Create();
        await TestDb.SeedHeroAsync(db);
        var service = new HeroService(db);
        var a = await service.AddStatAsync(new HeroStatInput { Type = "text", Value = "A", Label = "a" });
        var b = await service.AddStatAsync(new HeroStatInput { Type = "text", Value = "B", Label = "b" });
        var c = await service.AddStatAsync(new HeroStatInput { Type = "text", Value = "C", Label = "c" });

        await service.ReorderStatsAsync(new List<int> { c.Id, a.Id, b.Id });

        var hero = await service.GetHeroAsync();
        Assert.Equal(1, hero.Stats.First(s => s.Value == "C").SortOrder);
        Assert.Equal(2, hero.Stats.First(s => s.Value == "A").SortOrder);
        Assert.Equal(3, hero.Stats.First(s => s.Value == "B").SortOrder);
    }
}
