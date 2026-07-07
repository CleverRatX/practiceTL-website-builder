using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PracticeTL.Api.Dtos;
using PracticeTL.Api.Services;

namespace PracticeTL.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/offices")]
public class OfficeController : ControllerBase
{
    private readonly IOfficeService _service;
    public OfficeController(IOfficeService service) { _service = service; }

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _service.GetAllAsync());

    [HttpPost]
    public async Task<IActionResult> Add([FromBody] OfficeInput input)
    {
        try { return Ok(await _service.AddAsync(input)); }
        catch (ArgumentException ex) { return BadRequest(new { error = ex.Message }); }
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] OfficeInput input)
    {
        try
        {
            var o = await _service.UpdateAsync(id, input);
            return o is null ? NotFound() : Ok(o);
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
