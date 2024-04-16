using Microsoft.EntityFrameworkCore;
using Skills.Models;
using Skills.Models.CV;

namespace Skills.Databases;

public class SkillsContext : DbContext
{
    protected readonly IConfiguration Configuration;
    
    // CVs
    public DbSet<CvInfo> CVs { get; set; }
    public DbSet<CvEducationInfo> CvEducations { get; set; }
    public DbSet<CvExperienceInfo> CvExperiences { get; set; }
    public DbSet<CvCertificationInfo> CvCertifications { get; set; }
    public DbSet<CvSafetyCertificationInfo> CvSafetyCertifications { get; set; }
    
    public DbSet<SafetyCertification> SafetyCertifications { get; set; } // Admin certifications
    
    // Skills
    public DbSet<SkillModel> Skills { get; set; }
    public DbSet<SoftSkillModel> SoftSkills { get; set; }
    public DbSet<SKillInfo> SkillsTypes { get; set; }
    public DbSet<GroupModel> Groups { get; set; }
    
    // Users
    public DbSet<UserModel> Users { get; set; }
    public DbSet<UserSkillModel> UsersSkills { get; set; }
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
        
        // Skills models constraints
        modelBuilder.Entity<SkillModel>()
            .HasOne<SKillInfo>(e => e.TypeInfo).WithMany()
            .HasForeignKey(e => e.TypeId)
            .OnDelete(DeleteBehavior.Cascade);
        
        modelBuilder.Entity<SoftSkillModel>()
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
        
        modelBuilder.Entity<SoftSkillModel>()
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
        
        // CVs (drop cascade)
        modelBuilder.Entity<CvInfo>()
            .HasMany(e => e.Education)
            .WithOne()
            .OnDelete(DeleteBehavior.Cascade);
        
        modelBuilder.Entity<CvInfo>()
            .HasMany(e => e.Experiences)
            .WithOne()
            .OnDelete(DeleteBehavior.Cascade);
        
        modelBuilder.Entity<CvInfo>()
            .HasMany(e => e.Certifications)
            .WithOne()
            .OnDelete(DeleteBehavior.Cascade);
        
        modelBuilder.Entity<CvInfo>()
            .HasMany(e => e.SafetyCertifications)
            .WithOne()
            .OnDelete(DeleteBehavior.Cascade);
        
        // CVs (relationships)
        modelBuilder.Entity<SkillModel>()
            .HasOne(e => e.Cv).WithMany()
            .IsRequired(false)
            .HasForeignKey(e => e.CvId);
        
        modelBuilder.Entity<SoftSkillModel>()
            .HasOne(e => e.Cv).WithMany()
            .IsRequired(false)
            .HasForeignKey(e => e.CvId);
        
        modelBuilder.Entity<CvEducationInfo>()
            .HasOne(e => e.Cv).WithMany()
            .HasForeignKey(e => e.CvId);

        modelBuilder.Entity<CvExperienceInfo>()
            .HasOne(e => e.Cv).WithMany()
            .HasForeignKey(e => e.CvId);

        modelBuilder.Entity<CvCertificationInfo>()
            .HasOne(e => e.Cv).WithMany()
            .HasForeignKey(e => e.CvId);

        modelBuilder.Entity<CvSafetyCertificationInfo>()
            .HasOne(e => e.Cv).WithMany()
            .HasForeignKey(e => e.CvId);
        
        modelBuilder.Entity<CvSafetyCertificationInfo>()
            .HasOne(e => e.Certification).WithMany()
            .HasForeignKey(e => e.CertId);
    }
}