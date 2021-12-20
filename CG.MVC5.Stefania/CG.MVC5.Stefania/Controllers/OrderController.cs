using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CG.MVC5.Stefania.Models;

namespace CG.MVC5.Stefania.Controllers
{
    public class OrderController : Controller
    {
        // GET: Order
        [Route("Order")]
        [AuthorizeAccess]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult Index()
        {
            if (TempData.ContainsKey("msg"))
                ViewBag.Message = TempData["msg"];
            List<OrderProducts> orders = DBOperation.GetOrdersAccount((string)Session["email"]);
            return View(orders);
        }

        [AuthorizeAccess]
        public ActionResult DeleteOrder(int id_order)
        {

            if ((DBOperation.GetUserOfOrder(id_order) == (string)Session["email"]))
            {
                if (DBOperation.GetStateOfOrder(id_order) == "Confirmed")
                {
                    DBOperation.DeleteOrderFromUser(id_order);
                    TempData["msg"] = "showOrderDeleted";
                }    
                else
                    TempData["msg"] = "showStateOfOrder";
            }
            else
            {
                TempData["msg"] = "showUserOfOrder";
            }
            
            return RedirectToAction("Index", "Order");
        }

        [AuthorizeAccess]
        public ActionResult ConfirmOrder(int id_order)
        {
            if((DBOperation.GetUserOfOrder(id_order)) == (string)Session["email"])
            {
                if (DBOperation.GetStateOfOrder(id_order) == "Modified")
                {
                    DBOperation.ChangeStateOfOrder(id_order, "Confirmed");
                    TempData["msg"] = "showOrderConfirmed";
                }
                else
                    TempData["msg"] = "showStateOfOrder";
            }
            else
            {
                TempData["msg"] = "showUserOfOrder";
            }

            return RedirectToAction("Index", "Order");
        }

    }
}