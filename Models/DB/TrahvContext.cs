using System.Data.Entity;
using TrahvManage.Models.Account;

namespace TrahvManage.Models
{
    public class TrahvContext : DbContext
    {
        public DbSet<AccountModel> Accounts { get; set; }

        public DbSet<FineModel> Fines { get; set; }
        public DbSet<ChatModel> Chats { get; set; }
    }
}