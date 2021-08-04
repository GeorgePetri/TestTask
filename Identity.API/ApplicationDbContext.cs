using Identity.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace TestWeb.API
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext()
        {
        }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<UserEntity> Users { get; set; }

        public virtual DbSet<AddressEntity> Addresses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserEntity>()
                .HasOne(d => d.AddressEntity)
                .WithMany()
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}