using Microsoft.EntityFrameworkCore;
using PracticeTL.Api.Data;
using PracticeTL.Api.Dtos;
using PracticeTL.Api.Models;

namespace PracticeTL.Api.Services;

public class TeamService : ITeamService
{
    private readonly AppDbContext _db;

    public TeamService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<TeamMember>> GetAllAsync()
    {
        return await _db.TeamMembers
            .OrderBy(m => m.SortOrder)
            .ToListAsync();
    }

    public async Task<TeamMember> AddAsync(TeamMemberInput input)
    {
        Validate(input);

        var hasItems = await _db.TeamMembers.AnyAsync();
        var maxOrder = hasItems ? await _db.TeamMembers.MaxAsync(m => m.SortOrder) : 0;

        var member = new TeamMember
        {
            Name = input.Name,
            Position = input.Position,
            Photo = input.Photo,
            SortOrder = maxOrder + 1
        };

        _db.TeamMembers.Add(member);
        await _db.SaveChangesAsync();
        return member;
    }

    public async Task<TeamMember?> UpdateAsync(int id, TeamMemberInput input)
    {
        Validate(input);

        var member = await _db.TeamMembers.FindAsync(id);
        if (member is null)
            return null;

        member.Name = input.Name;
        member.Position = input.Position;
        member.Photo = input.Photo;
        await _db.SaveChangesAsync();
        return member;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var member = await _db.TeamMembers.FindAsync(id);
        if (member is null)
            return false;

        _db.TeamMembers.Remove(member);
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task ReorderAsync(List<int> orderedIds)
    {
        var members = await _db.TeamMembers.ToListAsync();
        for (int i = 0; i < orderedIds.Count; i++)
        {
            var member = members.FirstOrDefault(m => m.Id == orderedIds[i]);
            if (member != null)
                member.SortOrder = i + 1;
        }
        await _db.SaveChangesAsync();
    }

    private static void Validate(TeamMemberInput input)
    {
        if (string.IsNullOrWhiteSpace(input.Name))
            throw new ArgumentException("Имя не может быть пустым");
    }
}
