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

        public DbSet<UserEntity> Users { get; private set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var userBuilder = modelBuilder.Entity<UserEntity>();

            userBuilder
                .HasOne(d => d.AddressEntity)
                .WithOne()
                .HasForeignKey<UserEntity>("UserId")
                .OnDelete(DeleteBehavior.Cascade);
            userBuilder.HasIndex(u => u.Login)
                .IsUnique();

            var addressBuilder = modelBuilder.Entity<AddressEntity>();

            addressBuilder.Property<long>("UserId");
            addressBuilder.HasIndex(a => a.Country);
        }
    }
}