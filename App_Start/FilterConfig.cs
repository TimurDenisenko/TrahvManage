using System.Web;
using System.Web.Mvc;
using TrahvManage.Models;

namespace TrahvManage
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new UserState());
        }
    }
}
