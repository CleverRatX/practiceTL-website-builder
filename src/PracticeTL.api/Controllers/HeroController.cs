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

    // GET /api/hero  → весь блок: { title, subtitle, stats: [...] }
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var hero = await _service.GetHeroAsync();
        return Ok(hero);
    }

    // GET /api/hero => список всех элементов
    [HttpPut]
    public async Task<IActionResult> UpdateInfo([FromBody] HeroInfoInput input)
    {
        await _service.UpdateInfoAsync(input);
        return NoContent();
    }

    // POST /api/hero => создать элемент
    [HttpPost("stats")]
    public async Task<IActionResult> AddStat([FromBody] HeroStatInput input)
    {
        try
        {
            var stat = await _service.AddStatAsync(input);
            return Ok(stat);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    // PUT /api/hero/5  => изменить элемент
    [HttpPut("stats/{id:int}")]
    public async Task<IActionResult> UpdateStat(int id, [FromBody] HeroStatInput input)
    {
        try
        {
            var stat = await _service.UpdateStatAsync(id, input);
            if (stat is null) return NotFound();
            return Ok(stat);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    // DELETE /api/hero/5 => удалить по id
    [HttpDelete("stats/{id:int}")]
    public async Task<IActionResult> DeleteStat(int id)
    {
        var deleted = await _service.DeleteStatAsync(id);
        if (!deleted) return NotFound();
        return NoContent();
    }

    // POST /api/hero/reorder => новый порядок
    [HttpPost("stats/reorder")]
    public async Task<IActionResult> Reorder([FromBody] List<int> orderedIds)
    {
        await _service.ReorderStatsAsync(orderedIds);
        return NoContent();
    }
}
