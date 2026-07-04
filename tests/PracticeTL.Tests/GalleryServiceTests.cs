using PracticeTL.Api.Dtos;
using PracticeTL.Api.Services;
using Xunit;

namespace PracticeTL.Tests;

public class GalleryServiceTests
{
    [Fact]
    public async Task AddAsync_AssignsNextSortOrder()
    {
        using var db = TestDb.Create();
        var service = new GalleryService(db);

        var first = await service.AddAsync(new GalleryItemInput { Type = "img", ImageUrl = "/a.png", Caption = "первое" });
        var second = await service.AddAsync(new GalleryItemInput { Type = "img", ImageUrl = "/b.png", Caption = "второе" });

        Assert.True(first.Id > 0);
        Assert.Equal(1, first.SortOrder);
        Assert.Equal(2, second.SortOrder);
        Assert.Equal(2, (await service.GetAllAsync()).Count);
    }

    [Fact]
    public async Task AddAsync_EmptyImageUrl_Throws()
    {
        using var db = TestDb.Create();
        var service = new GalleryService(db);

        await Assert.ThrowsAsync<ArgumentException>(() =>
            service.AddAsync(new GalleryItemInput { Type = "img", ImageUrl = "", Caption = "x" }));
    }

    [Fact]
    public async Task AddAsync_DefaultsTypeToImg_WhenEmpty()
    {
        using var db = TestDb.Create();
        var service = new GalleryService(db);

        var item = await service.AddAsync(new GalleryItemInput { Type = "", ImageUrl = "/a.png", Caption = "x" });

        Assert.Equal("img", item.Type);
    }

    [Fact]
    public async Task AddAsync_KeepsVideoType()
    {
        using var db = TestDb.Create();
        var service = new GalleryService(db);

        var item = await service.AddAsync(new GalleryItemInput { Type = "video", ImageUrl = "/clip.mp4", Caption = "x" });

        Assert.Equal("video", item.Type);
    }

    [Fact]
    public async Task UpdateAsync_ChangesFields()
    {
        using var db = TestDb.Create();
        var service = new GalleryService(db);
        var g = await service.AddAsync(new GalleryItemInput { Type = "img", ImageUrl = "/old.png", Caption = "старое" });

        var updated = await service.UpdateAsync(g.Id, new GalleryItemInput { Type = "video", ImageUrl = "/new.mp4", Caption = "новое" });

        Assert.NotNull(updated);
        Assert.Equal("video", updated!.Type);
        Assert.Equal("/new.mp4", updated.ImageUrl);
        Assert.Equal("новое", updated.Caption);
    }

    [Fact]
    public async Task UpdateAsync_MissingId_ReturnsNull()
    {
        using var db = TestDb.Create();
        var service = new GalleryService(db);

        Assert.Null(await service.UpdateAsync(999, new GalleryItemInput { Type = "img", ImageUrl = "/x.png", Caption = "y" }));
    }

    [Fact]
    public async Task DeleteAsync_RemovesItem()
    {
        using var db = TestDb.Create();
        var service = new GalleryService(db);
        var g = await service.AddAsync(new GalleryItemInput { Type = "img", ImageUrl = "/x.png", Caption = "y" });

        Assert.True(await service.DeleteAsync(g.Id));
        Assert.Empty(await service.GetAllAsync());
    }

    [Fact]
    public async Task DeleteAsync_MissingId_ReturnsFalse()
    {
        using var db = TestDb.Create();
        var service = new GalleryService(db);

        Assert.False(await service.DeleteAsync(999));
    }

    [Fact]
    public async Task ReorderAsync_ChangesOrder()
    {
        using var db = TestDb.Create();
        var service = new GalleryService(db);
        var a = await service.AddAsync(new GalleryItemInput { Type = "img", ImageUrl = "/a.png", Caption = "a" });
        var b = await service.AddAsync(new GalleryItemInput { Type = "img", ImageUrl = "/b.png", Caption = "b" });
        var c = await service.AddAsync(new GalleryItemInput { Type = "img", ImageUrl = "/c.png", Caption = "c" });

        await service.ReorderAsync(new List<int> { c.Id, a.Id, b.Id });

        var all = await service.GetAllAsync();
        Assert.Equal("/c.png", all[0].ImageUrl);
        Assert.Equal("/a.png", all[1].ImageUrl);
        Assert.Equal("/b.png", all[2].ImageUrl);
    }
}
