using Microsoft.EntityFrameworkCore;
using Skills.Models;
using Skills.Models.CV;
using Skills.Models.Overview;

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
    public DbSet<CvSkillInfo> CvSkills { get; set; }
    
    public DbSet<SafetyCertification> SafetyCertifications { get; set; } // Admin certifications
    
    // Overview
    public DbSet<UserEducationInfo> UserEducations { get; set; }
    public DbSet<UserCertificationInfo> UserCertifications { get; set; }
    public DbSet<UserExperienceInfo> UserExperiences { get; set; }
    
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
        optionsBuilder.EnableSensitiveDataLogging();
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
        
        
        // CVs models constraints
        modelBuilder.Entity<UserEducationInfo>()
            .HasMany(e => e.CVs)
            .WithMany(e => e.Educations)
            .UsingEntity<CvEducationInfo>();
        
        modelBuilder.Entity<UserExperienceInfo>()
            .HasMany(e => e.CVs)
            .WithMany(e => e.Experiences)
            .UsingEntity<CvExperienceInfo>();
        
        modelBuilder.Entity<UserCertificationInfo>()
            .HasMany(e => e.CVs)
            .WithMany(e => e.Certifications)
            .UsingEntity<CvCertificationInfo>();
        
        modelBuilder.Entity<CvInfo>()
            .HasMany(e => e.Skills)
            .WithOne(e => e.Cv)
            .HasForeignKey(e => e.CvId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<CvSafetyCertificationInfo>()
            .HasOne<SafetyCertification>(e => e.Certification).WithMany()
            .HasForeignKey(e => e.CertId)
            .IsRequired();
    }
}