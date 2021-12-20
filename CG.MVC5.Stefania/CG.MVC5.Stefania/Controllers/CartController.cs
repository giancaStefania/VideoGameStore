using CG.MVC5.Stefania.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace CG.MVC5.Stefania.Controllers
{
    public class CartController : Controller
    {
        [AuthorizeAccess]
        [Route("Cart")]
        public ActionResult ShowCart()
        {
            ShoppingCart cart = DBOperation.GetShoppingCart((int)Session["cart"]);
            return View(cart);
        }
        [AuthorizeAccess]
        [HttpPost]
        public ActionResult AddCartElement(int id, int quantity, bool isHomeCart=false)
        {
            if (DBOperation.CheckIfProductIsInCart((int)Session["cart"], id) > 0)
                DBOperation.IncreaseProductInCart((int)Session["cart"], id, quantity);
            else
                DBOperation.AddProductToCart((int)Session["cart"], id, quantity);
            ShoppingCart cart = DBOperation.GetShoppingCart((int)Session["cart"]);
            if (isHomeCart)
                return PartialView("_RefreshHomeCart", cart);
            else
                return PartialView("_RefreshCart", cart);
        }
        [AuthorizeAccess]
        [HttpPost]
        public ActionResult DeleteProductFromCart(int id, bool isHomeCart)
        {
            DBOperation.DeleteProductFromCart((int)Session["cart"], id);
            ShoppingCart cart = DBOperation.GetShoppingCart((int)Session["cart"]);
            if (isHomeCart)
                return PartialView("_RefreshHomeCart", cart);
            else
                return PartialView("_RefreshCart", cart);
        }
        [AuthorizeAccess]
        [HttpPost]
        public ActionResult IncreaseDecreaseProductFromCart(int id, int quantity, bool isHomeCart)
        {
            if (DBOperation.GetQuantityOfProductCart((int)Session["cart"], id) + quantity > 0)
                DBOperation.IncreaseDecreaseProductFromCart((int)Session["cart"], id, quantity);
            else
            {
                if (isHomeCart)
                    DeleteProductFromCart(id, true);
                else
                    DeleteProductFromCart(id, false);
            }
            ShoppingCart cart = DBOperation.GetShoppingCart((int)Session["cart"]);
            if (isHomeCart)
                return PartialView("_RefreshHomeCart", cart);
            else
                return PartialView("_RefreshCart", cart);
        }
        public ActionResult GetCart()
        {
            ShoppingCart cart = DBOperation.GetShoppingCart(Session["cart"] != null ? (int)Session["cart"] : 0);
            return PartialView("_RefreshCart", cart);
        }

        [AuthorizeAccess]
        public ActionResult Checkout()
        {
            int id_order = DBOperation.CheckOutOrder((int)Session["cart"], (string)Session["email"]);
            if (id_order > 0)
            {
                MailOperation.SendMailOrder((string)Session["email"], "Your New Order", "EmailOrder.html", id_order, Request.Url.Authority);
                TempData["msg"] = "showCheckoutGood";
            }
            else
                TempData["msg"] = "showCheckoutBad";
            return RedirectToAction("Index", "Home");
        }
    }
}