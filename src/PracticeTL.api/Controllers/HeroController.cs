using Microsoft.AspNetCore.Mvc;
using PracticeTL.Api.Dtos;
using PracticeTL.Api.Services;

namespace PracticeTL.Api.Controllers;

[ApiController]
[Route("api/hero")]
public class HeroController : ControllerBase
{
    private readonly IHeroService _service;

    public HeroController(IHeroService service)
    {
        _service = service;
    }

    // GET /api/hero => список всех элементов
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var items = await _service.GetAllAsync();
        return Ok(items);
    }

    // GET /api/hero/5  => найти один элемент
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var item = await _service.GetByIdAsync(id);
        if (item is null)
            return NotFound();
        return Ok(item);
    }

    // POST /api/hero => создать элемент
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] HeroItemInput input)
    {
        try
        {
            var created = await _service.CreateAsync(input);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    // PUT /api/hero/5  => изменить элемент
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] HeroItemInput input)
    {
        try
        {
            var updated = await _service.UpdateAsync(id, input);
            if (updated is null)
                return NotFound();
            return Ok(updated);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    // DELETE /api/hero/5  => удалить по id
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _service.DeleteAsync(id);
        if (!deleted)
            return NotFound();
        return NoContent();
    }

    // POST /api/hero/reorder => новый порядок 
    // (тело: массив id, например [3,1,2])
    [HttpPost("reorder")]
    public async Task<IActionResult> Reorder([FromBody] List<int> orderedIds)
    {
        await _service.ReorderAsync(orderedIds);
        return NoContent();
    }
}
