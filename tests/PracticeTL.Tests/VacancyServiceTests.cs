using PracticeTL.Api.Dtos;
using PracticeTL.Api.Services;
using Xunit;

namespace PracticeTL.Tests;

public class VacancyServiceTests
{
    [Fact]
    public async Task AddAsync_ДобавляетВакансию_СоСледующимПорядком()
    {
        using var db = TestDb.Create();
        var service = new VacancyService(db);

        var first = await service.AddAsync(new VacancyInput { Title = "SRE", Address = "удаленно", Url = "https://hh.ru/1" });
        var second = await service.AddAsync(new VacancyInput { Title = "Аналитик", Address = "Йошкар-Ола", Url = "https://hh.ru/2" });

        Assert.True(first.Id > 0);
        Assert.Equal(1, first.SortOrder);
        Assert.Equal(2, second.SortOrder);
        Assert.Equal(2, (await service.GetAllAsync()).Count);
    }

    [Fact]
    public async Task AddAsync_ПустойЗаголовок_БросаетОшибку()
    {
        using var db = TestDb.Create();
        var service = new VacancyService(db);

        await Assert.ThrowsAsync<ArgumentException>(() =>
            service.AddAsync(new VacancyInput { Title = "", Address = "удаленно", Url = "https://hh.ru" }));
    }

    [Fact]
    public async Task UpdateAsync_МеняетПоля()
    {
        using var db = TestDb.Create();
        var service = new VacancyService(db);
        var v = await service.AddAsync(new VacancyInput { Title = "Старое", Address = "x", Url = "/x" });

        var updated = await service.UpdateAsync(v.Id, new VacancyInput { Title = "Новое", Address = "y", Url = "/y" });

        Assert.NotNull(updated);
        Assert.Equal("Новое", updated!.Title);
        Assert.Equal("y", updated.Address);
        Assert.Equal("/y", updated.Url);
    }

    [Fact]
    public async Task UpdateAsync_НесуществующийId_ВозвращаетNull()
    {
        using var db = TestDb.Create();
        var service = new VacancyService(db);

        Assert.Null(await service.UpdateAsync(999, new VacancyInput { Title = "x", Address = "y", Url = "/z" }));
    }

    [Fact]
    public async Task DeleteAsync_УдаляетВакансию()
    {
        using var db = TestDb.Create();
        var service = new VacancyService(db);
        var v = await service.AddAsync(new VacancyInput { Title = "x", Address = "y", Url = "/z" });

        Assert.True(await service.DeleteAsync(v.Id));
        Assert.Empty(await service.GetAllAsync());
    }

    [Fact]
    public async Task DeleteAsync_НесуществующийId_ВозвращаетFalse()
    {
        using var db = TestDb.Create();
        var service = new VacancyService(db);

        Assert.False(await service.DeleteAsync(999));
    }

    [Fact]
    public async Task ReorderAsync_ПереставляетПорядок()
    {
        using var db = TestDb.Create();
        var service = new VacancyService(db);
        var a = await service.AddAsync(new VacancyInput { Title = "A", Address = "x", Url = "/a" });
        var b = await service.AddAsync(new VacancyInput { Title = "B", Address = "x", Url = "/b" });
        var c = await service.AddAsync(new VacancyInput { Title = "C", Address = "x", Url = "/c" });

        await service.ReorderAsync(new List<int> { c.Id, a.Id, b.Id });

        var all = await service.GetAllAsync();
        Assert.Equal("C", all[0].Title);
        Assert.Equal("A", all[1].Title);
        Assert.Equal("B", all[2].Title);
    }
}
