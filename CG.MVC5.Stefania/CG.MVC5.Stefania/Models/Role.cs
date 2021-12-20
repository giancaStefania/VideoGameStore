using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CG.MVC5.Stefania.Models
{
    public class Role : FilterAttribute, IAuthorizationFilter
    {
        private string Name;
        public Role(string name)
        {
            this.Name = name;
        }
        public void OnAuthorization(AuthorizationContext filterContext)
        {
            filterContext.Controller.ViewBag.Role = Name;
            if (filterContext.HttpContext.Session["email"] != null)
            {
                string email = filterContext.HttpContext.Session["email"].ToString();
                string roles = DBOperation.GetRole(email);
                if (roles != Name)
                    filterContext.Result = new RedirectResult("/");
            }
            else
            {
                filterContext.Result = new RedirectResult("/");
            }
        }
    }
}