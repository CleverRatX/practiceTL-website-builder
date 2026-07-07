using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PracticeTL.Api.Dtos;
using PracticeTL.Api.Services;

namespace PracticeTL.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/benefits")]
public class BenefitController : ControllerBase
{
    private readonly IBenefitService _service;
    public BenefitController(IBenefitService service) { _service = service; }

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _service.GetAllAsync());

    [HttpPost]
    public async Task<IActionResult> Add([FromBody] BenefitInput input)
    {
        try { return Ok(await _service.AddAsync(input)); }
        catch (ArgumentException ex) { return BadRequest(new { error = ex.Message }); }
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] BenefitInput input)
    {
        try
        {
            var b = await _service.UpdateAsync(id, input);
            return b is null ? NotFound() : Ok(b);
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
