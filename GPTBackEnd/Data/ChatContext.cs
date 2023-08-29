using GPTBackEnd.Model;
using Microsoft.EntityFrameworkCore;

namespace GPTBackEnd.Data
{
    public class ChatContext : DbContext
    {
        public ChatContext() { }

        public ChatContext(DbContextOptions<ChatContext> options) : base(options) { }
        
        // 具体的表，连接哪个实体
        public DbSet<Chat> Chats { get; set; }

        // 连接数据库
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            string endpointURL = "https://gptava.documents.azure.com:443/";
            string primaryKey = "BW3wsteDWaDWjnmvxL8E7BVCFrGrap5aSRpAsVcADmmKULLexzuq8cbyddpaLW6unsHPsXnSZ7C7ACDbnM9hrA==";
            string databaseName = "avagpt";
            optionsBuilder.UseCosmos(endpointURL, primaryKey, databaseName);
        }

        // 从哪里加载entity type configuration
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Chat>()
                .ToContainer("Chat")
                .HasPartitionKey(e => e.Id);
        }
    }
}
