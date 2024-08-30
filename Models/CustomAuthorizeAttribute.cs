using System.Web.Mvc;

namespace TrahvManage.Models
{
    public class CustomAuthorizeAttribute : AuthorizeAttribute
    {
        public static class Authorized
        {
            public static bool IsUserAuthorized(string userName)
            {
                return false;
            }
        }
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            filterContext.Result = new RedirectToRouteResult(
                new System.Web.Routing.RouteValueDictionary(
                    new { controller = "Account", action = "Login" }
                )
            );
        }
    }
}