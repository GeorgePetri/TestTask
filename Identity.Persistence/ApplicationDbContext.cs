using Identity.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Identity.Persistence
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<UserEntity> Users { get; set; }

        public DbSet<AddressEntity> Addresses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserEntity>()
                .HasOne(d => d.AddressEntity)
                .WithMany()
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}