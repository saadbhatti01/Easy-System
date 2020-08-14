using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Routing;

namespace EasySystem.Models
{
    public class SessionCheckAttribute : ActionFilterAttribute
    {
        private static HttpContextAccessor _httpContextAccessor;
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            int? ID = filterContext.HttpContext.Session.GetInt32("ID");
            if (ID == null)
            {
                //FormsAuthentication.SignOut();
                filterContext.Result =
               new RedirectToRouteResult(new RouteValueDictionary
                        {
                              { "action", "SessionOut" },
                            { "controller", "Users" }
                         });
                return;
            }
        }
    }

    public class SessionCheckForAdminAttribute : ActionFilterAttribute
    {
        private static HttpContextAccessor _httpContextAccessor;
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            int? ID = filterContext.HttpContext.Session.GetInt32("ID");
            if (ID == null)
            {
                int? Role = filterContext.HttpContext.Session.GetInt32("RoleId");
                if (Role != 1)
                {
                    filterContext.Result =
               new RedirectToRouteResult(new RouteValueDictionary
                        {
                              { "action", "Logout" },
                            { "controller", "Users" }
                         });
                    return;
                }
            }
        }
    }

    public class SessionCheckPublicAttribute : ActionFilterAttribute
    {
        private static HttpContextAccessor _httpContextAccessor;
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            int? ID = filterContext.HttpContext.Session.GetInt32("AdminID");
            if (ID == null)
            {
                int? Role = filterContext.HttpContext.Session.GetInt32("Role");
                if (Role != 1)
                {
                    filterContext.Result =
               new RedirectToRouteResult(new RouteValueDictionary
                        {
                              { "action", "Logout" },
                            { "controller", "Home" }
                         });
                    return;
                }
            }
        }
    }

}