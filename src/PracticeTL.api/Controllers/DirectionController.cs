using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PracticeTL.Api.Dtos;
using PracticeTL.Api.Services;

namespace PracticeTL.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/directions")]
public class DirectionController : ControllerBase
{
    private readonly IDirectionService _service;
    public DirectionController(IDirectionService service) { _service = service; }

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _service.GetAllAsync());

    [HttpPost]
    public async Task<IActionResult> Add([FromBody] DirectionInput input)
    {
        try { return Ok(await _service.AddAsync(input)); }
        catch (ArgumentException ex) { return BadRequest(new { error = ex.Message }); }
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] DirectionInput input)
    {
        try
        {
            var d = await _service.UpdateAsync(id, input);
            return d is null ? NotFound() : Ok(d);
        }
        catch (ArgumentException ex) { return BadRequest(new { error = ex.Message }); }
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
        => await _service.DeleteAsync(id) ? NoContent() : NotFound();

    [HttpPost("reorder")]
    public async Task<IActionResult> Reorder([FromBody] List<int> orderedIds)
    {
        await _service.ReorderAsync(orderedIds);
        return NoContent();
    }
}
