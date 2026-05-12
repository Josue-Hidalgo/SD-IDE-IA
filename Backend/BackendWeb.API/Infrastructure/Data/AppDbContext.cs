using Microsoft.EntityFrameworkCore;
using BackendWeb.API.Domain.Entities;

namespace BackendWeb.API.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Student> Students => Set<Student>();
    public DbSet<Professor> Professors => Set<Professor>();

    protected override void OnModelCreating(ModelBuilder mb)
    {
        mb.Entity<User>()
          .HasIndex(u => u.EmailUser)
          .IsUnique();
    }
}