using Microsoft.EntityFrameworkCore;
using Skills.Models;

namespace Skills.Databases;

public class SkillsContext : DbContext
{
    protected readonly IConfiguration Configuration;
    
    public DbSet<SkillModel> Skills { get; set; }
    public DbSet<UserModel> Users { get; set; }
    public DbSet<UserSkillModel> Userskills { get; set; }
    public DbSet<SKillInfo> SkillInfos { get; set; }

    public SkillsContext(DbContextOptions<SkillsContext> options, IConfiguration configuration) : base(options)
    {
        Configuration = configuration;
    }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite(Configuration.GetConnectionString("SkillsDb"));
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserModel>()
            .HasMany(e => e.Skills)
            .WithMany(e => e.Users)
            .UsingEntity<UserSkillModel>();
    }
}