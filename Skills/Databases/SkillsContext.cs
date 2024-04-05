using Microsoft.EntityFrameworkCore;
using Skills.Models;

namespace Skills.Databases;

public class SkillsContext : DbContext
{
    protected readonly IConfiguration Configuration;
    
    public DbSet<SkillModel> Skills { get; set; }
    public DbSet<SoftSkillModel> SoftSkills { get; set; }
    public DbSet<SKillInfo> SkillsTypes { get; set; }
    public DbSet<GroupModel> Groups { get; set; }
    
    public DbSet<UserModel> Users { get; set; }
    public DbSet<UserSkillModel> Userskills { get; set; }
    public DbSet<TypeLevel> TypesLevels { get; set; }
    public DbSet<SoftTypeLevel> SoftTypesLevels { get; set; }

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
        // UserModel constraints
        modelBuilder.Entity<UserModel>()
            .HasOne<GroupModel>(e => e.Group).WithMany()
            .HasForeignKey(e => e.GroupId)
            .IsRequired(false);

        modelBuilder.Entity<UserSkillModel>()
            .HasKey(e => new { e.UserId, e.SkillId });
        
        // SkillModel constraints
        modelBuilder.Entity<SkillModel>()
            .HasOne<SKillInfo>(e => e.TypeInfo).WithMany()
            .HasForeignKey(e => e.TypeId)
            .OnDelete(DeleteBehavior.Cascade);
        
        modelBuilder.Entity<SkillModel>()
            .HasOne<SKillInfo>(e => e.CategoryInfo).WithMany()
            .HasForeignKey(e => e.CategoryId)
            .OnDelete(DeleteBehavior.Cascade);
        
        modelBuilder.Entity<SkillModel>()
            .HasOne<SKillInfo>(e => e.SubCategoryInfo).WithMany()
            .HasForeignKey(e => e.SubCategoryId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<SkillModel>()
            .HasOne<GroupModel>(e => e.Group).WithMany()
            .HasForeignKey(e => e.GroupId)
            .IsRequired(false);
        
        // Skills type levels
        modelBuilder.Entity<TypeLevel>()
            .HasOne(e => e.Type).WithMany()
            .HasForeignKey(e => e.TypeId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<SoftTypeLevel>()
            .HasOne(e => e.Skill).WithMany()
            .HasForeignKey(e => e.SkillId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}