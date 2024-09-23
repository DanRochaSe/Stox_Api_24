using Microsoft.EntityFrameworkCore;
using webapi.Entities;

namespace webapi.Helpers
{
    public class ConnectContext : DbContext
    {
        public readonly IConfiguration Configuration;

        public ConnectContext(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public ConnectContext(DbContextOptions<ConnectContext> options) : base(options)
        {


        }

        public DbSet<Post> Posts { get; set; } = null!;

    }
}
