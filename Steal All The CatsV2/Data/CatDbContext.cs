using Microsoft.EntityFrameworkCore;
using Steal_All_The_CatsV2.Models;

namespace Steal_All_The_CatsV2.Data;

public class CatDbContext(DbContextOptions<CatDbContext> options) : DbContext(options)
{
    public DbSet<CatEntity> Cats { get; set; }
    public DbSet<TagEntity> Tags { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Cat entity
        modelBuilder.Entity<CatEntity>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.CatId)
                  .IsRequired();

            entity.Property(e => e.Width);

            entity.Property(e => e.Height);

            entity.Property(e => e.Image)
                  .IsRequired();

            entity.Property(e => e.Created)
                  .HasColumnType("datetime2");
        });

        // Tag entity
        modelBuilder.Entity<TagEntity>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Name)
                  .IsRequired()
                  .HasMaxLength(50);

            entity.Property(e => e.Created)
                  .HasDefaultValueSql("GETUTCDATE()");
        });

        // Many-to-many relationship using EF conventions
        modelBuilder.Entity<CatEntity>()
            .HasMany(c => c.Tags)
            .WithMany(t => t.Cats)
            .UsingEntity(j => j.ToTable("CatTag"));
    }
}
