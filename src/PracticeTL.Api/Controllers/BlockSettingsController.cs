using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PracticeTL.Api.Dtos;
using PracticeTL.Api.Services;

namespace PracticeTL.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/blocks")]
public class BlockSettingsController : ControllerBase
{
    private readonly IBlockSettingService _service;

    public BlockSettingsController(IBlockSettingService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _service.GetAllAsync());
    }

    [HttpPut("{key}")]
    public async Task<IActionResult> SetVisible(string key, [FromBody] BlockVisibilityInput input)
    {
        var updated = await _service.SetVisibleAsync(key, input.Visible);
        return updated is null ? NotFound() : Ok(updated);
    }
}
