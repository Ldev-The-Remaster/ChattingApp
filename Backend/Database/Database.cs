using Backend.Models.Messages;
using Backend.Models.Users;
using Microsoft.EntityFrameworkCore;

namespace Backend.Database
{
    public class TextMessageContext : DbContext
    {
        public DbSet<TextMessage> TextMessages { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite($"Data Source=Messages.db");
        }

        public static void SetUp() 
        {
            TextMessageContext messageContext = new TextMessageContext();
            // This vvv should be disabled in prod
            // messageContext.Database.EnsureDeleted();
            messageContext.Database.EnsureCreated();
        }
    }

    public class UserContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite($"Data Source=Users.db");
        }

        public static void SetUp()
        {
            UserContext userContext = new UserContext();
            // This vvv should be disabled in prod
            // userContext.Database.EnsureDeleted();
            userContext.Database.EnsureCreated();
        }
    }

    public class BannedIpContext : DbContext
    {
        public DbSet<BannedIp> BannedIps { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite($"Data Source=BannedIps.db");
        }

        public static void SetUp()
        {
            BannedIpContext bannedIpContext = new BannedIpContext();
            // This vvv should be disabled in prod
            // userContext.Database.EnsureDeleted();
            bannedIpContext.Database.EnsureCreated();
        }
    }
}
