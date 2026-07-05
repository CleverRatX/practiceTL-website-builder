using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PracticeTL.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/upload")]
public class UploadController : ControllerBase
{
    private readonly IWebHostEnvironment _env;

    private static readonly string[] Allowed =
        { ".png", ".jpg", ".jpeg", ".gif", ".webp", ".svg", ".mp4", ".webm" };

    public UploadController(IWebHostEnvironment env) { _env = env; }

    [HttpPost]
    public async Task<IActionResult> Upload(IFormFile? file)
    {
        if (file is null || file.Length == 0)
            return BadRequest(new { error = "Файл не выбран" });

        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!Allowed.Contains(ext))
            return BadRequest(new { error = "Недопустимый тип файла" });

        var webRoot = _env.WebRootPath ?? Path.Combine(_env.ContentRootPath, "wwwroot");
        var folder = Path.Combine(webRoot, "media", "uploads");
        Directory.CreateDirectory(folder);

        var name = $"{Guid.NewGuid():N}{ext}";
        var fullPath = Path.Combine(folder, name);
        using (var stream = System.IO.File.Create(fullPath))
        {
            await file.CopyToAsync(stream);
        }

        return Ok(new { url = $"/media/uploads/{name}" });
    }
}
