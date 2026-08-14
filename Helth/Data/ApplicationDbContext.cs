using Microsoft.EntityFrameworkCore;
using Helth.Models;

namespace Helth.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Admin> Admins => Set<Admin>();
    public DbSet<Employee> Employees => Set<Employee>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Admin>()
            .HasIndex(a => a.Username)
            .IsUnique();

        modelBuilder.Entity<Employee>()
            .Property(e => e.Gender)
            .HasConversion<string>()
            .HasMaxLength(20);
    }
}
