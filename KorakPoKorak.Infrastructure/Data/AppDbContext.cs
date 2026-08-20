using System.Text.Json;
using KorakPoKorak.Domain;
using KorakPoKorak.Domain.Entities;
using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<Workshop> Workshops { get; set; }
    public DbSet<Lesson> Lessons { get; set; }
    public DbSet<Exercise> Exercises { get; set; }
    public DbSet<Quiz> Quizzes { get; set; }
    public DbSet<Question> Questions { get; set; }
    public DbSet<Answer> Answers { get; set; }
    public DbSet<ChildProfile> ChildProfiles { get; set; }
    public DbSet<Enrollment> Enrollments { get; set; }
    public DbSet<ActivityProgress> ActivityProgresses { get; set; }
    public DbSet<MediaAsset> MediaAssets { get; set; }
    public DbSet<ContentBlock> ContentBlocks { get; set; }
    public DbSet<BadgeTemplate> BadgeTemplates { get; set; }
    public DbSet<BadgeAward> BadgeAwards { get; set; }
    public DbSet<CertificateTemplate> CertificateTemplates { get; set; }
    public DbSet<CertificateAward> CertificateAwards { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User → Role (many-to-one)
        modelBuilder.Entity<User>()
            .HasOne(u => u.Role)
            .WithMany(r => r.Users)
            .HasForeignKey(u => u.RoleId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<User>()
            .HasIndex(u => u.ActivationToken)
            .IsUnique();

        modelBuilder.Entity<User>()
            .HasIndex(u => u.PasswordResetToken)
            .IsUnique();

        // Lesson → User (created by)
        modelBuilder.Entity<Lesson>()
            .HasOne(l => l.CreatedBy)
            .WithMany()
            .HasForeignKey(l => l.CreatedById)
            .OnDelete(DeleteBehavior.Restrict);

        // Exercise → User (created by)
        modelBuilder.Entity<Exercise>()
            .HasOne(e => e.CreatedBy)
            .WithMany()
            .HasForeignKey(e => e.CreatedById)
            .OnDelete(DeleteBehavior.Restrict);

        // Workshop → User (created by)
        modelBuilder.Entity<Workshop>()
            .HasOne(w => w.CreatedBy)
            .WithMany()
            .HasForeignKey(w => w.CreatedById)
            .OnDelete(DeleteBehavior.Restrict);

        // Workshop ↔ Lesson (many-to-many)
        modelBuilder.Entity<Workshop>()
            .HasMany(w => w.Lessons)
            .WithMany(l => l.Workshops)
            .UsingEntity("WorkshopLessons");

        // Workshop ↔ Exercise (many-to-many)
        modelBuilder.Entity<Workshop>()
            .HasMany(w => w.Exercises)
            .WithMany(e => e.Workshops)
            .UsingEntity("WorkshopExercises");

        // Workshop ↔ User (contributors, many-to-many)
        modelBuilder.Entity<Workshop>()
            .HasMany(w => w.Contributors)
            .WithMany()
            .UsingEntity("WorkshopContributors");

        // Quiz → User (created by)
        modelBuilder.Entity<Quiz>()
            .HasOne(qz => qz.CreatedBy)
            .WithMany()
            .HasForeignKey(qz => qz.CreatedById)
            .OnDelete(DeleteBehavior.Restrict);

        // Exercise → Quiz (optional, reusable)
        modelBuilder.Entity<Exercise>()
            .HasOne(e => e.Quiz)
            .WithMany()
            .HasForeignKey(e => e.QuizId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Exercise>()
            .HasIndex(e => e.QuizId);

        // Question → Quiz (cascade delete)
        modelBuilder.Entity<Question>()
            .HasOne(qq => qq.Quiz)
            .WithMany(qz => qz.Questions)
            .HasForeignKey(qq => qq.QuizId)
            .OnDelete(DeleteBehavior.Cascade);

        // Answer → Question (cascade delete)
        modelBuilder.Entity<Answer>()
            .HasOne(a => a.Question)
            .WithMany(qq => qq.Answers)
            .HasForeignKey(a => a.QuestionId)
            .OnDelete(DeleteBehavior.Cascade);

        // Store Workshop.ActivityTypes as JSON
        modelBuilder.Entity<Workshop>()
            .Property(w => w.ActivityTypes)
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions?)null) ?? new List<string>()
            )
            .HasColumnType("jsonb");

        // ChildProfile → User (parent, one-to-many)
        modelBuilder.Entity<ChildProfile>()
            .HasOne(c => c.Parent)
            .WithMany()
            .HasForeignKey(c => c.ParentId)
            .OnDelete(DeleteBehavior.Cascade);

        // Enrollment → ChildProfile (many-to-one)
        modelBuilder.Entity<Enrollment>()
            .HasOne(e => e.ChildProfile)
            .WithMany(c => c.Enrollments)
            .HasForeignKey(e => e.ChildProfileId)
            .OnDelete(DeleteBehavior.Cascade);

        // Enrollment → Workshop (many-to-one)
        modelBuilder.Entity<Enrollment>()
            .HasOne(e => e.Workshop)
            .WithMany(w => w.Enrollments)
            .HasForeignKey(e => e.WorkshopId)
            .OnDelete(DeleteBehavior.Restrict);

        // One enrollment per ChildProfile + Workshop
        modelBuilder.Entity<Enrollment>()
            .HasIndex(e => new { e.ChildProfileId, e.WorkshopId })
            .IsUnique();

        // ActivityProgress → Enrollment (many-to-one)
        modelBuilder.Entity<ActivityProgress>()
            .HasOne(p => p.Enrollment)
            .WithMany(e => e.ActivityProgresses)
            .HasForeignKey(p => p.EnrollmentId)
            .OnDelete(DeleteBehavior.Cascade);

        // One progress row per Enrollment + UnitType + UnitId
        modelBuilder.Entity<ActivityProgress>()
            .HasIndex(p => new { p.EnrollmentId, p.UnitType, p.UnitId })
            .IsUnique();

        // MediaAsset → User (created by)
        modelBuilder.Entity<MediaAsset>()
            .HasOne(m => m.CreatedBy)
            .WithMany()
            .HasForeignKey(m => m.CreatedById)
            .OnDelete(DeleteBehavior.Restrict);

        // ContentBlock → Lesson (optional)
        modelBuilder.Entity<ContentBlock>()
            .HasOne(b => b.Lesson)
            .WithMany(l => l.ContentBlocks)
            .HasForeignKey(b => b.LessonId)
            .OnDelete(DeleteBehavior.Cascade);

        // ContentBlock → Exercise (optional)
        modelBuilder.Entity<ContentBlock>()
            .HasOne(b => b.Exercise)
            .WithMany(e => e.ContentBlocks)
            .HasForeignKey(b => b.ExerciseId)
            .OnDelete(DeleteBehavior.Cascade);

        // ContentBlock → MediaAsset (required)
        modelBuilder.Entity<ContentBlock>()
            .HasOne(b => b.MediaAsset)
            .WithMany()
            .HasForeignKey(b => b.MediaAssetId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<ContentBlock>()
            .HasIndex(b => b.LessonId);

        modelBuilder.Entity<ContentBlock>()
            .HasIndex(b => b.ExerciseId);

        modelBuilder.Entity<ContentBlock>()
            .ToTable(t => t.HasCheckConstraint(
                "CK_ContentBlocks_OneOwner",
                "(\"LessonId\" IS NOT NULL AND \"ExerciseId\" IS NULL) OR (\"LessonId\" IS NULL AND \"ExerciseId\" IS NOT NULL)"));

        ConfigureBadgeAndCertificateModel(modelBuilder);

        // Seed roles
        modelBuilder.Entity<Role>().HasData(
            new Role { Id = 1, RoleName = UserRole.Administrator, Description = "System administrator with full access to all features." },
            new Role { Id = 2, RoleName = UserRole.Mentor, Description = "Mentor who guides and supports children on the platform." },
            new Role { Id = 3, RoleName = UserRole.Child, Description = "Child user of the platform." },
            new Role { Id = 4, RoleName = UserRole.Parent, Description = "Parent who manages their children's profiles and activities." }
        );
    }

    private static void ConfigureBadgeAndCertificateModel(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BadgeTemplate>(e =>
        {
            e.HasIndex(t => t.Code).IsUnique();
            e.Property(t => t.Code).HasMaxLength(64).IsRequired();
            e.Property(t => t.Name).HasMaxLength(128).IsRequired();
            e.Property(t => t.Description).HasMaxLength(512).IsRequired();
            e.Property(t => t.IconUrl).HasMaxLength(256).IsRequired();
        });

        modelBuilder.Entity<BadgeAward>(e =>
        {
            e.HasOne(a => a.BadgeTemplate)
                .WithMany(t => t.Awards)
                .HasForeignKey(a => a.BadgeTemplateId)
                .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(a => a.ChildProfile)
                .WithMany(c => c.BadgeAwards)
                .HasForeignKey(a => a.ChildProfileId)
                .OnDelete(DeleteBehavior.Cascade);

            e.HasOne(a => a.Workshop)
                .WithMany(w => w.BadgeAwards)
                .HasForeignKey(a => a.WorkshopId)
                .OnDelete(DeleteBehavior.SetNull);

            e.HasOne(a => a.Enrollment)
                .WithMany(en => en.BadgeAwards)
                .HasForeignKey(a => a.EnrollmentId)
                .OnDelete(DeleteBehavior.SetNull);

            e.HasIndex(a => a.ChildProfileId);
            e.HasIndex(a => new { a.ChildProfileId, a.BadgeTemplateId })
                .IsUnique()
                .HasFilter("\"Scope\" = 0")
                .HasDatabaseName("IX_BadgeAwards_OncePerChild");
            e.HasIndex(a => new { a.ChildProfileId, a.BadgeTemplateId, a.WorkshopId })
                .IsUnique()
                .HasFilter("\"Scope\" = 1 AND \"WorkshopId\" IS NOT NULL")
                .HasDatabaseName("IX_BadgeAwards_OncePerChildWorkshop");
        });

        modelBuilder.Entity<CertificateTemplate>(e =>
        {
            e.HasIndex(t => t.Code).IsUnique();
            e.Property(t => t.Code).HasMaxLength(64).IsRequired();
            e.Property(t => t.Title).HasMaxLength(128).IsRequired();
            e.Property(t => t.Description).HasMaxLength(512).IsRequired();
        });

        modelBuilder.Entity<CertificateAward>(e =>
        {
            e.HasOne(a => a.CertificateTemplate)
                .WithMany(t => t.Awards)
                .HasForeignKey(a => a.CertificateTemplateId)
                .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(a => a.ChildProfile)
                .WithMany(c => c.CertificateAwards)
                .HasForeignKey(a => a.ChildProfileId)
                .OnDelete(DeleteBehavior.Cascade);

            e.HasOne(a => a.Workshop)
                .WithMany(w => w.CertificateAwards)
                .HasForeignKey(a => a.WorkshopId)
                .OnDelete(DeleteBehavior.SetNull);

            e.HasOne(a => a.Enrollment)
                .WithOne(en => en.CertificateAward)
                .HasForeignKey<CertificateAward>(a => a.EnrollmentId)
                .OnDelete(DeleteBehavior.Cascade);

            e.HasIndex(a => a.EnrollmentId).IsUnique();
            e.HasIndex(a => a.CertificateNumber).IsUnique();
            e.HasIndex(a => a.ChildProfileId);
            e.Property(a => a.CertificateNumber).HasMaxLength(64).IsRequired();
            e.Property(a => a.ChildFullName).HasMaxLength(256).IsRequired();
            e.Property(a => a.WorkshopTitle).HasMaxLength(256).IsRequired();
            e.Property(a => a.MentorName).HasMaxLength(256).IsRequired();
            e.Property(a => a.CertificateTitle).HasMaxLength(128).IsRequired();
        });

        SeedBadgeAndCertificateTemplates(modelBuilder);
    }

    private static void SeedBadgeAndCertificateTemplates(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BadgeTemplate>().HasData(
            Template(1, BadgeCodes.WorkshopMaster, "Majstor radionice",
                "Dodeli se za svaku uspešno završenu radionicu.",
                BadgeCategory.Workshop, BadgeAwardScope.OncePerChildWorkshop),
            Template(2, BadgeCodes.FirstWorkshop, "Prvi korak",
                "Dodeli se za prvu uspešno završenu radionicu.",
                BadgeCategory.Milestone, BadgeAwardScope.OncePerChild),
            Template(3, BadgeCodes.Persistent, "Upornost",
                "Dodeli se za drugu uspešno završenu radionicu.",
                BadgeCategory.Milestone, BadgeAwardScope.OncePerChild),
            Template(4, BadgeCodes.ThreeWorkshops, "Iskusni istraživač",
                "Dodeli se za treću uspešno završenu radionicu.",
                BadgeCategory.Milestone, BadgeAwardScope.OncePerChild),
            Template(5, BadgeCodes.CuriousMind, "Radoznali um",
                "Dodeli se za četvrtu uspešno završenu radionicu.",
                BadgeCategory.Milestone, BadgeAwardScope.OncePerChild),
            Template(6, BadgeCodes.FiveWorkshops, "Veliki istraživač",
                "Dodeli se za petu uspešno završenu radionicu.",
                BadgeCategory.Milestone, BadgeAwardScope.OncePerChild),
            Template(7, BadgeCodes.GreatProgress, "Veliki napredak",
                "Dodeli se za šestu uspešno završenu radionicu.",
                BadgeCategory.Milestone, BadgeAwardScope.OncePerChild),
            Template(8, BadgeCodes.Dedicated, "Posvećeni učenik",
                "Dodeli se za sedmu uspešno završenu radionicu.",
                BadgeCategory.Milestone, BadgeAwardScope.OncePerChild),
            Template(9, BadgeCodes.Explorer, "Istraživač",
                "Dodeli se za osmu uspešno završenu radionicu.",
                BadgeCategory.Milestone, BadgeAwardScope.OncePerChild),
            Template(10, BadgeCodes.TenWorkshops, "Avanturista",
                "Dodeli se za desetu uspešno završenu radionicu.",
                BadgeCategory.Milestone, BadgeAwardScope.OncePerChild),
            Template(11, BadgeCodes.SuperLearner, "Super učenik",
                "Dodeli se za petnaestu uspešno završenu radionicu.",
                BadgeCategory.Milestone, BadgeAwardScope.OncePerChild),
            Template(12, BadgeCodes.Champion, "Šampion",
                "Dodeli se za dvadesetu uspešno završenu radionicu.",
                BadgeCategory.Milestone, BadgeAwardScope.OncePerChild),
            Template(13, BadgeCodes.KnowledgeStar, "Zvezda znanja",
                "Dodeli se za prvu završenu radionicu srednje težine.",
                BadgeCategory.Complexity, BadgeAwardScope.OncePerChild),
            Template(14, BadgeCodes.BraveStep, "Hrabri korak",
                "Dodeli se za prvu završenu naprednu radionicu.",
                BadgeCategory.Complexity, BadgeAwardScope.OncePerChild),
            Template(15, BadgeCodes.CreativeStar, "Kreativna zvezda",
                "Dodeli se za prvu završenu radionicu sa više vrsta aktivnosti.",
                BadgeCategory.Workshop, BadgeAwardScope.OncePerChild)
        );

        modelBuilder.Entity<CertificateTemplate>().HasData(
            new CertificateTemplate
            {
                Id = 1,
                Code = CertificateCodes.DefaultWorkshop,
                Title = "Diploma",
                Description = "Potvrda o uspešno završenoj radionici."
            }
        );
    }

    private static BadgeTemplate Template(
        int id,
        string code,
        string name,
        string description,
        BadgeCategory category,
        BadgeAwardScope scope) => new()
    {
        Id = id,
        Code = code,
        Name = name,
        Description = description,
        IconUrl = $"/badges/{code.ToLowerInvariant()}.svg",
        Category = category,
        Scope = scope
    };
}
