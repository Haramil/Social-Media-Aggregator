using Microsoft.AspNet.Identity.EntityFramework;
using SearchLibrary;
using System.Data.Entity;

namespace AggregatorServer.Models
{
    public class ApplicationContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationContext()
            : base("DefaultConnection")
        { }

        public static ApplicationContext Create()
        {
            return new ApplicationContext();
        }
        public DbSet<GeneralPost> Posts { get; set; }
        public DbSet<Pagination> Paginations { get; set; }
    }
}