
using System.Web.Mvc;
using System.Web.Routing;

namespace CG.MVC5.Stefania.Models
{
    public class WithoutAuthorize : FilterAttribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationContext filterContext)
        {
            if (filterContext.HttpContext.Session["email"] != null)
                filterContext.Result = new RedirectToRouteResult(
                                   new RouteValueDictionary
                                   {
                                       { "action", "Index" },
                                       { "controller", "Home" }
                                   });
        }
    }

}