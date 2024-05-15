using Backend.Models.Messages;
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
}
