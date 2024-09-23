using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using webapi.Entities;

namespace webapi.Data
{
    public class StoxContext :DbContext
    {
        public StoxContext(DbContextOptions<StoxContext> options) : base(options) 
        {
        }

        public  DbSet<User> Users { get; set; }
        public DbSet<Goals> Goals { get; set; }
        public DbSet<Post> Posts { get; set; }

        public DbSet<UserSalt> UserSalts { get; set; }

        public DbSet<Stox> Stox { get; set; }

        public DbSet<StoxComparison> StoxComparison { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            

            modelBuilder.Entity<User>().ToTable("User");
            modelBuilder.Entity<Goals>().ToTable("Goal");
            modelBuilder.Entity<Post>().ToTable("Post");
            modelBuilder.Entity<UserSalt>().ToTable("UserSalts");
            modelBuilder.Entity<Stox>().ToTable("Stox");
            modelBuilder.Entity<StoxComparison>().ToTable("StoxComparison");

        }



    }
}
