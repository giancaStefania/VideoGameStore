using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using CG.MVC5.Stefania.Models;

namespace CG.MVC5.Stefania.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult Index(int page = 1, string category = null, string keyWord = null, string orderBy = null, string platform=null)
        {
            CheckCookieIsSet();
            List<ProductE> allProducts = DBOperation.GetAllProduct(page, category, keyWord, orderBy, platform);
            if (TempData.ContainsKey("msg"))
                ViewBag.Message = TempData["msg"];
            var numOfProduct = DBOperation.GetNumberOfPage(category, keyWord, orderBy, platform);
            ViewBag.NumPage = numOfProduct;
            ViewBag.PageCount = Math.Ceiling(numOfProduct/ (double)4);
            ViewBag.CurrentPage = page;
            ViewBag.KeyWord = keyWord;
            ViewBag.Category = category;
            ViewBag.OrderBy = orderBy;
            ViewBag.Platform = platform;
            return View(allProducts);
        }

        private void CheckCookieIsSet()
        {
            if (Request.Cookies.Count >= 4)
            {
                User user = DBOperation.CheckLogin(Request.Cookies["email"].Value, Request.Cookies["password"].Value);
                if (user != null)
                {
                    Session["password"] = user.Password;
                    Session["name"] = user.Name;
                    Session["surname"] = user.Surname;
                    Session["email"] = user.Email;
                    Session["cart"] = Int32.Parse(Request.Cookies["cart"].Value);
                }
            }
        }

        [Role("admin")]
        public ActionResult AddProduct()
        {
            return View();

        }

        [Role("admin")]
        [HttpPost]
        public ActionResult AddProduct(ProductE p)
        {
            try
            {
                if (ModelState.IsValid)
                
                {
                    if (CheckImage(p))
                    {
                        if (CheckHtml(p))
                        {
                            DBOperation.SetProduct(p);
                            TempData["msg"] = "showAddProduct";
                            return RedirectToAction("Index", "Home");
                        }
                    }
                }
                return View();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return View();
            }
        }
        [Role("admin")]
        [Route("Home/Edit/{id}")]
        public ActionResult EditProduct(int id)
        {
            ProductE product = DBOperation.GetSingleProduct(id);
            return View(product);
        }
        [Role("admin")]
        [Route("Home/Edit/{id}")]
        [HttpPost]
        public ActionResult EditProduct(ProductE p)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (CheckImageEdit(p))
                    {
                        if (CheckHtml(p))
                        {
                            DBOperation.UpdateProduct(p);
                            TempData["msg"] = "showEditProduct";
                            return RedirectToAction("Index", "Home");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("Image1_file", "The files must be an image and size must be under 5 MB");
                    }
                }
                return View(p);

            } catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return View(p);
            }


        }
        [Role("admin")]
        [Route("Home/Delete/{id}")]
        public ActionResult DeleteProduct(int id)
        {
            DBOperation.DeleteProduct(id);
            CheckProductDeletedInOrders(id);
            TempData["msg"] = "showDeleteProduct";
            return RedirectToAction("Index", "Home");
        }

        [Route("Home/Details/{id}")]
        public ActionResult DetailsProduct(int id)
        {
            ProductE product = DBOperation.GetSingleProduct(id);
            ViewBag.LastAction = GetPreviousAction();
            return View(product);
        }
        private string GetPreviousAction()
        {
            try
            {
                string url = System.Web.HttpContext.Current.Request.UrlReferrer.PathAndQuery != null ? System.Web.HttpContext.Current.Request.UrlReferrer.PathAndQuery : "/";

                return url;
            }catch(Exception e)
            {
                Console.WriteLine(e.Message);
                return "/";
            }
        }
        private bool CheckImage(ProductE p)
        {
            try
            {
                if (p.Image1_file != null)
                {
                    if (!(CheckTypeImageEdit(p.Image1_file) && CheckTypeImageEdit(p.Image2_file) && CheckTypeImageEdit(p.Image3_file)))
                    {
                        ModelState.AddModelError("Image1_file", "The product must be and image and size must be under 5 MB");
                        return false;
                    }
                    return true;

                }
                else
                {
                    ModelState.AddModelError("Image1_file", "The product must contain at least one image");
                    return false;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }
        private bool CheckImageEdit(ProductE p)
        {
            try
            {
                List<string> images = DBOperation.InitializeOldImageForEditProduct(p.Id);
                bool flag = true;
                if ((images[0] != p.Image1_string) && (p.Image1_file != null) && !CheckTypeImageEdit(p.Image1_file))
                {
                    flag = flag && false;
                    ModelState.AddModelError("Image1", "The product must contain at least one image");
                }
                if ((images[1] != p.Image2_string && (p.Image2_file != null) && !CheckTypeImageEdit(p.Image2_file)))
                    flag = flag && false;
                if ((images[2] != p.Image3_string) && (p.Image3_file != null) && !CheckTypeImageEdit(p.Image3_file))
                    flag = flag && false;
                return flag;
            } catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }

        }
        private bool CheckTypeImageEdit(HttpPostedFileBase image)
        {
            if (image != null)
            {
                if ((!(image.ContentType == "image/jpeg" || image.ContentType == "image/png" || image.ContentType == "image/jpg")) || !(image.ContentLength <= 5242880))
                    return false;
                else
                    return true;
            }
            return true;


        }
        [Role("admin")]
        public ActionResult GetCategories(string category = null)
        {
            ViewBag.category = category;
            return PartialView(DBOperation.GetCategories());
        }
        [Role("admin")]
        public ActionResult GetStock(string stock = null)
        {
            ViewBag.stock = stock;
            return PartialView(DBOperation.GetStock());
        }
        [Role("admin")]
        public ActionResult GetPlatform(string platform = null, bool isPrincipal = true)
        {
            ViewBag.platform = platform;
            ViewBag.pricipal = isPrincipal;
            return PartialView(DBOperation.GetPlatform());
        }
        [Role("admin")]
        public ActionResult AddPlatform(string platform)
        {
            try
            {
                DBOperation.AddNewPlatform(platform);
                return RedirectToAction("GetPlatform", "Home");
            }catch(Exception e)
            {
                Console.WriteLine(e.Message);
                return PartialView("GetPlatform", "Home");
            }

        }
        [Role("admin")]
        public ActionResult DeletePlatform(string platform)
        {
            try
            {
                DBOperation.DeletePlatform(platform);
                return RedirectToAction("GetPlatform", "Home");
            }catch(Exception e)
            {
                Console.WriteLine(e.Message);
                return RedirectToAction("GetPlatform", "Home");
            }
        }
        private bool CheckHtml(ProductE p)
        {
            try
            {
                bool flag = true;
                Regex reg = new Regex(@"<[^>]+>");

                var prova = reg.IsMatch(p.Description);
                if (reg.IsMatch(p.Name))
                {
                    ModelState.AddModelError("Name", "The title must not contain html tag");
                    flag = flag && false;
                }
                if (reg.IsMatch(p.Price))
                {
                    ModelState.AddModelError("Price", "The price must not contain html tag");
                    flag = flag && false;
                }
                if (reg.IsMatch(p.Description))
                {
                    ModelState.AddModelError("Description", "The description must not contain html tag");
                    flag = flag && false;
                }
                return flag;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }

        private void CheckProductDeletedInOrders(int id_product)
        {
            try
            {
                List<int> orders = DBOperation.CheckOrderContainProduct(id_product);
                foreach (int order in orders)
                {
                    if (CheckOrderContainOnlyOneProduct(order)) { 
                        DBOperation.ChangeStateOfOrder(order, "Cancelled");
                    }else {
                        DBOperation.ChangeStateOfOrder(order, "Modified");
                    }
                    MailOperation.SendMailProductDeleted(DBOperation.GetUserOfOrder(order), "Your order is changed", "EmailOrderInvalidate.html", order, DBOperation.GetSingleProduct(id_product), Request.Url.Authority);
                }
            }catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        private bool CheckOrderContainOnlyOneProduct(int id_order)
        {
            List<ProductE> products = DBOperation.GetProductsOfOrder(id_order);
            if (products.Count() == 1)
                return true;
            else
                return false;
        }
      
    }
}