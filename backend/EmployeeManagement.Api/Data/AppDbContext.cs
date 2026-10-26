using EmployeeManagement.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Employee> Employees => Set<Employee>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // No two employees can share an email (database-level safety net).
        modelBuilder.Entity<Employee>()
            .HasIndex(e => e.Email)
            .IsUnique();

        // Store the enum as readable text ("Active"/"Inactive") instead of a number (0/1),
        // so the data makes sense when you look at it in SSMS.
        modelBuilder.Entity<Employee>()
            .Property(e => e.Status)
            .HasConversion<string>()
            .HasMaxLength(20);
    }
}
