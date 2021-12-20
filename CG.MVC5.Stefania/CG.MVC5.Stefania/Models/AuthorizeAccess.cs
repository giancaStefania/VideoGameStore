using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CG.MVC5.Stefania.Models
{
    public class AuthorizeAccess : FilterAttribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationContext filterContext)
        {
            if (filterContext.HttpContext.Session["email"] == null)
                filterContext.Result = new RedirectResult("/");
        }
    }
}