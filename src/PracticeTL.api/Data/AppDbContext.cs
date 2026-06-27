using Microsoft.EntityFrameworkCore;
using PracticeTL.Api.Models;

namespace PracticeTL.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<HeroItem> HeroItems => Set<HeroItem>();
    
    // \public DbSet<TeamMember> TeamMembers => Set<TeamMember>();
}
