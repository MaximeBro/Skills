using Microsoft.EntityFrameworkCore;
using Skills.Models;

namespace Skills.Databases;

public class SkillsContext : DbContext
{
    protected readonly IConfiguration Configuration;
    
    public DbSet<SkillModel> Skills { get; set; }

    public SkillsContext(IConfiguration configuration)
    {
        Configuration = configuration;
    }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite(Configuration.GetConnectionString("SkillsDb"));
    }
}