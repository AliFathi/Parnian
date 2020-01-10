using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;

namespace Parnian.Models
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        //public ApplicationDbContext() : base("ParnianLocalDb", throwIfV1Schema: false) { }
        public ApplicationDbContext() : base("ParnianDb", throwIfV1Schema: false) { }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        public DbSet<Page> Pages { get; set; }

        public DbSet<Slide> Slides { get; set; }

        public DbSet<Video> Videos { get; set; }

        public DbSet<Journal> Journals { get; set; }

        public DbSet<Graphic> Graphics { get; set; }

        public DbSet<Setting> Settings { get; set; }

        public DbSet<Category> Categories { get; set; }
    }
}