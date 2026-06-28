using Microsoft.EntityFrameworkCore;
using PracticeTL.Api.Models;

namespace PracticeTL.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Hero> Heroes => Set<Hero>();
    public DbSet<HeroStat> HeroStats => Set<HeroStat>();
    public DbSet<TeamMember> TeamMembers => Set<TeamMember>();
}
