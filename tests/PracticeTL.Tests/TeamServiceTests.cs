using PracticeTL.Api.Dtos;
using PracticeTL.Api.Services;
using Xunit;

namespace PracticeTL.Tests;

public class TeamServiceTests
{
    [Fact]
    public async Task AddAsync_ДобавляетСотрудника_СоСледующимПорядком()
    {
        using var db = TestDb.Create();
        var service = new TeamService(db);

        var first = await service.AddAsync(new TeamMemberInput { Name = "Алексей", Position = "CTO", Photo = "/a.png" });
        var second = await service.AddAsync(new TeamMemberInput { Name = "Юра", Position = "CPO", Photo = "/b.png" });

        Assert.True(first.Id > 0);
        Assert.Equal(1, first.SortOrder);
        Assert.Equal(2, second.SortOrder);
        Assert.Equal(2, (await service.GetAllAsync()).Count);
    }

    [Fact]
    public async Task AddAsync_ПустоеИмя_БросаетОшибку()
    {
        using var db = TestDb.Create();
        var service = new TeamService(db);

        await Assert.ThrowsAsync<ArgumentException>(() =>
            service.AddAsync(new TeamMemberInput { Name = "", Position = "x", Photo = "/x.png" }));
    }

    [Fact]
    public async Task UpdateAsync_МеняетПоля()
    {
        using var db = TestDb.Create();
        var service = new TeamService(db);
        var m = await service.AddAsync(new TeamMemberInput { Name = "Старое", Position = "x", Photo = "/x.png" });

        var updated = await service.UpdateAsync(m.Id, new TeamMemberInput { Name = "Новое", Position = "y", Photo = "/y.png" });

        Assert.NotNull(updated);
        Assert.Equal("Новое", updated!.Name);
        Assert.Equal("y", updated.Position);
        Assert.Equal("/y.png", updated.Photo);
    }

    [Fact]
    public async Task UpdateAsync_НесуществующийId_ВозвращаетNull()
    {
        using var db = TestDb.Create();
        var service = new TeamService(db);

        Assert.Null(await service.UpdateAsync(999, new TeamMemberInput { Name = "x", Position = "y", Photo = "/z.png" }));
    }

    [Fact]
    public async Task DeleteAsync_УдаляетСотрудника()
    {
        using var db = TestDb.Create();
        var service = new TeamService(db);
        var m = await service.AddAsync(new TeamMemberInput { Name = "x", Position = "y", Photo = "/z.png" });

        Assert.True(await service.DeleteAsync(m.Id));
        Assert.Empty(await service.GetAllAsync());
    }

    [Fact]
    public async Task DeleteAsync_НесуществующийId_ВозвращаетFalse()
    {
        using var db = TestDb.Create();
        var service = new TeamService(db);

        Assert.False(await service.DeleteAsync(999));
    }

    [Fact]
    public async Task ReorderAsync_ПереставляетПорядок()
    {
        using var db = TestDb.Create();
        var service = new TeamService(db);
        var a = await service.AddAsync(new TeamMemberInput { Name = "A", Position = "p", Photo = "/a.png" });
        var b = await service.AddAsync(new TeamMemberInput { Name = "B", Position = "p", Photo = "/b.png" });
        var c = await service.AddAsync(new TeamMemberInput { Name = "C", Position = "p", Photo = "/c.png" });

        await service.ReorderAsync(new List<int> { c.Id, a.Id, b.Id });

        var all = await service.GetAllAsync();
        Assert.Equal("C", all[0].Name);
        Assert.Equal("A", all[1].Name);
        Assert.Equal("B", all[2].Name);
    }
}
