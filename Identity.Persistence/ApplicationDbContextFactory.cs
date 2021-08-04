using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Identity.Persistence
{
    /// <summary>
    /// This class is added in order to have migrations in Identity.Persistence and get rid of the parameterless constructor on <see cref="ApplicationDbContext"/> .
    /// </summary>
    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>();
            options.UseSqlServer("Data Source=.");
            return new ApplicationDbContext(options.Options);
        }
    }
}