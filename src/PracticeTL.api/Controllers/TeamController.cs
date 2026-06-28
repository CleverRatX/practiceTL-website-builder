using Microsoft.AspNetCore.Mvc;
using PracticeTL.Api.Dtos;
using PracticeTL.Api.Services;

namespace PracticeTL.Api.Controllers;

[ApiController]
[Route("api/team")]
public class TeamController : ControllerBase
{
    private readonly ITeamService _service;

    public TeamController(ITeamService service)
    {
        _service = service;
    }

    // GET /api/team => список сотрудников
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var members = await _service.GetAllAsync();
        return Ok(members);
    }

    // POST /api/team => добавить
    [HttpPost]
    public async Task<IActionResult> Add([FromBody] TeamMemberInput input)
    {
        try
        {
            var member = await _service.AddAsync(input);
            return Ok(member);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    // PUT /api/team/5 => изменить
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] TeamMemberInput input)
    {
        try
        {
            var member = await _service.UpdateAsync(id, input);
            if (member is null) return NotFound();
            return Ok(member);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    // DELETE /api/team/5 => удалить
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _service.DeleteAsync(id);
        if (!deleted) return NotFound();
        return NoContent();
    }

    // POST /api/team/reorder => новый порядок (тело: массив id)
    [HttpPost("reorder")]
    public async Task<IActionResult> Reorder([FromBody] List<int> orderedIds)
    {
        await _service.ReorderAsync(orderedIds);
        return NoContent();
    }
}
