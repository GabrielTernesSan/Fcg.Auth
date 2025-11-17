using Fcg.Auth.Infra.Tables;
using Microsoft.EntityFrameworkCore;

namespace Fcg.Auth.Infra
{
    public class FcgAuthDbContext : DbContext
    {
        public FcgAuthDbContext(DbContextOptions<FcgAuthDbContext> options) : base(options) { }

        public virtual DbSet<User> Users { get; set; }
    }
}
