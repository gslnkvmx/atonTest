using Microsoft.EntityFrameworkCore;
using atonTest.Models;

namespace atonTest.Data;

public class ApplicationDbContext : DbContext
{
  public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
      : base(options)
  {
  }

  public DbSet<User> Users { get; set; }

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    base.OnModelCreating(modelBuilder);

    modelBuilder.Entity<User>(entity =>
    {
      entity.HasKey(e => e.Id);
      entity.Property(e => e.Login).IsRequired();
      entity.HasIndex(e => e.Login).IsUnique();
      entity.Property(e => e.Password).IsRequired();
      entity.Property(e => e.Name).IsRequired();
      entity.Property(e => e.Gender).IsRequired();
      entity.Property(e => e.CreatedOn).IsRequired();
      entity.Property(e => e.CreatedBy).IsRequired();
      entity.Property(e => e.ModifiedOn).IsRequired();
      entity.Property(e => e.ModifiedBy).IsRequired();
    });
  }
}