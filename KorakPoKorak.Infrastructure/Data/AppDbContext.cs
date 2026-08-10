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
    public DbSet<ChildProfile> ChildProfiles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User → Role (many-to-one)
        modelBuilder.Entity<User>()
            .HasOne(u => u.Role)
            .WithMany(r => r.Users)
            .HasForeignKey(u => u.RoleId)
            .OnDelete(DeleteBehavior.Restrict);

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

        // Seed roles
        modelBuilder.Entity<Role>().HasData(
            new Role { Id = 1, RoleName = UserRole.Administrator, Description = "System administrator with full access to all features." },
            new Role { Id = 2, RoleName = UserRole.Mentor, Description = "Mentor who guides and supports children on the platform." },
            new Role { Id = 3, RoleName = UserRole.Child, Description = "Child user of the platform." },
            new Role { Id = 4, RoleName = UserRole.Parent, Description = "Parent who manages their children's profiles and activities." }
        );
    }
}
