using System;
using System.Web.Mvc;
using System.Web.Routing;

namespace TrahvManage.Models
{
    public class UserState : ActionFilterAttribute
    {
        public static int Id { get; set; }
        public static string Name { get; set; }
        public static bool Authorized { get; set; }
        public static string Role { get; set; }
        private string _requiredRole;
        public UserState()
        {
            _requiredRole = null;
        }
        public UserState(string requiredRole = "User")
        {
            _requiredRole = requiredRole;
        }
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (!Authorized
                && filterContext.ActionDescriptor.ActionName != "Register"
                && filterContext.ActionDescriptor.ActionName != "Login"
                && filterContext.ActionDescriptor.ActionName != "Recovery")
            {
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Account", action = "Register" }));
                return;
            }
            if (Role != null && _requiredRole != null && Role != _requiredRole)
            {
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Home", action = "Index" }));
                return;
            }
        }
    }
}