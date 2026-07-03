using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PracticeTL.Api.Dtos;
using PracticeTL.Api.Services;

namespace PracticeTL.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/brands")]
public class BrandController : ControllerBase
{
    private readonly IBrandService _service;
    public BrandController(IBrandService service) { _service = service; }

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _service.GetAllAsync());

    [HttpPost]
    public async Task<IActionResult> Add([FromBody] BrandInput input)
    {
        try { return Ok(await _service.AddAsync(input)); }
        catch (ArgumentException ex) { return BadRequest(new { error = ex.Message }); }
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] BrandInput input)
    {
        try
        {
            var brand = await _service.UpdateAsync(id, input);
            return brand is null ? NotFound() : Ok(brand);
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
