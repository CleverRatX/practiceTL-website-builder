using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;
using PracticeTL.Api.Controllers;
using Xunit;

namespace PracticeTL.Tests;

public class MediaControllerTests
{
    private sealed class FakeEnv : IWebHostEnvironment
    {
        public string WebRootPath { get; set; } = "";
        public IFileProvider WebRootFileProvider { get; set; } = null!;
        public string ApplicationName { get; set; } = "tests";
        public IFileProvider ContentRootFileProvider { get; set; } = null!;
        public string ContentRootPath { get; set; } = "";
        public string EnvironmentName { get; set; } = "Test";
    }

    private static IFormFile FakeFile(string fileName, string content)
    {
        var bytes = Encoding.UTF8.GetBytes(content);
        return new FormFile(new MemoryStream(bytes), 0, bytes.Length, "file", fileName);
    }

    private static (FakeEnv env, string media) NewEnv()
    {
        var webroot = Path.Combine(Path.GetTempPath(), "tlmedia_" + Guid.NewGuid().ToString("N"));
        var media = Path.Combine(webroot, "media");
        Directory.CreateDirectory(media);
        return (new FakeEnv { WebRootPath = webroot }, media);
    }

    [Fact]
    public void List_ReturnsFoldersAndFiles()
    {
        var (env, media) = NewEnv();
        Directory.CreateDirectory(Path.Combine(media, "team"));
        File.WriteAllText(Path.Combine(media, "logo.png"), "x");
        var controller = new MediaController(env);

        var result = controller.List("") as OkObjectResult;

        Assert.NotNull(result);
        var json = JsonSerializer.Serialize(result!.Value);
        Assert.Contains("team", json);
        Assert.Contains("logo.png", json);
    }

    [Fact]
    public void List_Traversal_ReturnsBadRequest()
    {
        var (env, _) = NewEnv();
        var controller = new MediaController(env);

        var result = controller.List("../../etc");

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task Upload_SavesFileIntoMedia()
    {
        var (env, media) = NewEnv();
        var controller = new MediaController(env);

        var result = await controller.Upload("", FakeFile("photo.png", "data"));

        Assert.IsType<OkObjectResult>(result);
        Assert.True(File.Exists(Path.Combine(media, "photo.png")));
    }

    [Fact]
    public async Task Upload_DisallowedExtension_ReturnsBadRequest()
    {
        var (env, _) = NewEnv();
        var controller = new MediaController(env);

        var result = await controller.Upload("", FakeFile("evil.exe", "data"));

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void Delete_RemovesFile()
    {
        var (env, media) = NewEnv();
        var path = Path.Combine(media, "del.png");
        File.WriteAllText(path, "x");
        var controller = new MediaController(env);

        var result = controller.Delete("del.png");

        Assert.IsType<NoContentResult>(result);
        Assert.False(File.Exists(path));
    }

    [Fact]
    public void Delete_Traversal_ReturnsBadRequest()
    {
        var (env, _) = NewEnv();
        var controller = new MediaController(env);

        var result = controller.Delete("../secret.txt");

        Assert.IsType<BadRequestObjectResult>(result);
    }
}
