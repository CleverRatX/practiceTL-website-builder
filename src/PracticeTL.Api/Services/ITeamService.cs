using PracticeTL.Api.Dtos;
using PracticeTL.Api.Models;

namespace PracticeTL.Api.Services;

public interface ITeamService
{
    Task<List<TeamMember>> GetAllAsync();
    Task<TeamMember> AddAsync(TeamMemberInput input);
    Task<TeamMember?> UpdateAsync(int id, TeamMemberInput input);
    Task<bool> DeleteAsync(int id);
    Task ReorderAsync(List<int> orderedIds);
}
