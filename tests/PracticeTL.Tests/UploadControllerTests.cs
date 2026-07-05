using System.Text;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;
using PracticeTL.Api.Controllers;
using Xunit;

namespace PracticeTL.Tests;

public class UploadControllerTests
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
        var stream = new MemoryStream(bytes);
        return new FormFile(stream, 0, bytes.Length, "file", fileName);
    }

    private static string TempDir()
    {
        var dir = Path.Combine(Path.GetTempPath(), "tltests_" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(dir);
        return dir;
    }

    [Fact]
    public async Task Upload_NoFile_ReturnsBadRequest()
    {
        var controller = new UploadController(new FakeEnv { WebRootPath = TempDir() });

        var result = await controller.Upload(null);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task Upload_DisallowedExtension_ReturnsBadRequest()
    {
        var controller = new UploadController(new FakeEnv { WebRootPath = TempDir() });

        var result = await controller.Upload(FakeFile("evil.exe", "x"));

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task Upload_ValidImage_SavesFileAndReturnsOk()
    {
        var dir = TempDir();
        var controller = new UploadController(new FakeEnv { WebRootPath = dir });

        var result = await controller.Upload(FakeFile("photo.png", "imagebytes"));

        Assert.IsType<OkObjectResult>(result);
        var files = Directory.GetFiles(Path.Combine(dir, "media", "uploads"));
        Assert.Single(files);
        Assert.EndsWith(".png", files[0]);
    }
}
