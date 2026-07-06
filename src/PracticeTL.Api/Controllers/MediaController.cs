using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PracticeTL.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/media")]
public class MediaController : ControllerBase
{
    private readonly IWebHostEnvironment _env;

    private static readonly string[] ImageExt =
        { ".png", ".jpg", ".jpeg", ".gif", ".webp", ".svg" };

    private static readonly string[] AllowedUpload =
        { ".png", ".jpg", ".jpeg", ".gif", ".webp", ".svg", ".mp4", ".webm" };

    public MediaController(IWebHostEnvironment env) { _env = env; }

    private string Root()
    {
        var webRoot = _env.WebRootPath ?? Path.Combine(_env.ContentRootPath, "wwwroot");
        return Path.GetFullPath(Path.Combine(webRoot, "media"));
    }

    private string? Resolve(string? rel)
    {
        rel = (rel ?? "").Replace('\\', '/').Trim('/');
        var root = Root();
        var full = Path.GetFullPath(Path.Combine(root, rel));
        if (full != root && !full.StartsWith(root + Path.DirectorySeparatorChar))
            return null;
        return full;
    }

    private static string UrlFor(string rel, string name)
        => "/media/" + (rel == "" ? "" : rel + "/") + name;

    [HttpGet("list")]
    public IActionResult List(string? path)
    {
        var full = Resolve(path);
        if (full is null || !Directory.Exists(full))
            return BadRequest(new { error = "Папка не найдена" });

        var rel = (path ?? "").Replace('\\', '/').Trim('/');

        var folders = Directory.GetDirectories(full)
            .Select(d => Path.GetFileName(d))
            .OrderBy(n => n)
            .ToList();

        var files = Directory.GetFiles(full)
            .Select(f => Path.GetFileName(f))
            .OrderBy(n => n)
            .Select(name => new
            {
                name,
                url = UrlFor(rel, name),
                isImage = ImageExt.Contains(Path.GetExtension(name).ToLowerInvariant())
            })
            .ToList();

        return Ok(new { path = rel, folders, files });
    }

    [HttpPost("upload")]
    public async Task<IActionResult> Upload([FromForm] string? path, IFormFile? file)
    {
        if (file is null || file.Length == 0)
            return BadRequest(new { error = "Файл не выбран" });

        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!AllowedUpload.Contains(ext))
            return BadRequest(new { error = "Недопустимый тип файла" });

        var folder = Resolve(path);
        if (folder is null || !Directory.Exists(folder))
            return BadRequest(new { error = "Папка не найдена" });

        var name = Path.GetFileName(file.FileName);
        var dest = Path.Combine(folder, name);
        using (var stream = System.IO.File.Create(dest))
        {
            await file.CopyToAsync(stream);
        }

        var rel = (path ?? "").Replace('\\', '/').Trim('/');
        return Ok(new { url = UrlFor(rel, name) });
    }

    [HttpDelete]
    public IActionResult Delete(string? path)
    {
        var full = Resolve(path);
        if (full is null || !System.IO.File.Exists(full))
            return BadRequest(new { error = "Файл не найден" });

        System.IO.File.Delete(full);
        return NoContent();
    }
}
