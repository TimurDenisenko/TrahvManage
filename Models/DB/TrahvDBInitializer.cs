using System.Data.Entity;

namespace TrahvManage.Models
{
    public class TrahvDBInitializer : CreateDatabaseIfNotExists<TrahvContext>
    {
        protected override void Seed(TrahvContext db)
        {
            base.Seed(db);
        }
    }
}