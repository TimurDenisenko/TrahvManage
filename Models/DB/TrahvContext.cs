using System.Data.Entity;
using TrahvManage.Models.Account;

namespace TrahvManage.Models
{
    public class TrahvContext : DbContext
    {
        public DbSet<AccountModel> Accounts { get; set; }
    }
}