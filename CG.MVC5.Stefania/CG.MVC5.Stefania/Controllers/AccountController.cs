using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.UI;
using CG.MVC5.Stefania.Models;


namespace CG.MVC5.Stefania.Controllers
{
    public class AccountController : Controller
    {
     
        [WithoutAuthorize]
        public ActionResult LoginPage()
        {
            DisableBackButton();
            return View();
        }

        [WithoutAuthorize]
        [HttpPost]
        public ActionResult LoginPage(User us, string remember)
        {
            if (ModelState.IsValidField("Email") && ModelState.IsValidField("Password"))
            {
                string password = Encryptor.MD5Hash(us.Password);
                User login = DBOperation.CheckLogin(us.Email, password);
                if(login != null)
                {
                    Session["name"] = login.Name;
                    Session["surname"] = login.Surname;
                    Session["email"] = login.Email;
                    Session["password"] = login.Password;
                    Session["role"] = DBOperation.GetRole(login.Email);
                    CheckShoppingCart(login.Email);
                    CheckRememberMe(remember);
                    DBOperation.InvalidateToken(login.Email);
                    TempData["msg"] = "showLogin";
                    return RedirectToAction("Index", "Home");
                }
                ViewBag.Error = "Match not correct";
            }
            return View(us);
        }

        private void CheckShoppingCart(string email)
        {
            try
            {
                int idCart = DBOperation.CheckShoppingCart(email);
                if (idCart == 0)
                    idCart = DBOperation.CreateShoppingCart(email);
                Session["Cart"] = idCart;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        private void CheckRememberMe(string remember)
        {
            try
            {
                if (remember == "on")
                {
                    Response.Cookies["email"].Value = Session["email"].ToString();
                    Response.Cookies["email"].Expires = DateTime.Now.AddDays(7);

                    Response.Cookies["name"].Value = Session["name"].ToString();
                    Response.Cookies["name"].Expires = DateTime.Now.AddDays(7);

                    Response.Cookies["surname"].Value = Session["surname"].ToString();
                    Response.Cookies["surname"].Expires = DateTime.Now.AddDays(7);

                    Response.Cookies["password"].Value = Session["password"].ToString();
                    Response.Cookies["password"].Expires = DateTime.Now.AddDays(7);

                    Response.Cookies["role"].Value = Session["role"].ToString();
                    Response.Cookies["role"].Expires = DateTime.Now.AddDays(7);

                    Response.Cookies["cart"].Value = Session["cart"].ToString();
                    Response.Cookies["cart"].Expires = DateTime.Now.AddDays(7);
                }
                else
                {
                    if (Request.Cookies["email"] != null)
                    {
                        ClearCookies();
                    }
                }
            }catch(Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        [AuthorizeAccess]
        public ActionResult Logout()
        {
            Session.Clear();
            ClearCookies();
            TempData["msg"] = "showLogout";
            return RedirectToAction("Index","Home");
        }
        [WithoutAuthorize]
        public ActionResult ForgotPassword()
        {
            return View();
        }
        [WithoutAuthorize]
        [HttpPost]
        public ActionResult ForgotPassword(User u)
        {
            if (ModelState.IsValidField("Email") && DBOperation.EmailExist(u.Email))
            {
                if (DBOperation.CheckEmailToken(u.Email))
                    DBOperation.InvalidateToken(u.Email);
                
                string token = GenerateToken(u.Email);
                DBOperation.SetToken(u.Email, token);
                sendMail(u.Email, token, Request.Url.Authority); 
            }
            TempData["msg"] = "'showSuccess'";
            return RedirectToAction("LoginPage", "Account");
            
        }
        private void ClearCookies()
        {
            try
            {
                Response.Cookies["email"].Expires = DateTime.Now.AddYears(-1);
                Response.Cookies["name"].Expires = DateTime.Now.AddYears(-1);
                Response.Cookies["surname"].Expires = DateTime.Now.AddYears(-1);
                Response.Cookies["password"].Expires = DateTime.Now.AddYears(-1);
                Response.Cookies["role"].Expires = DateTime.Now.AddYears(-1);
            }
            catch(Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private void DisableBackButton()
        {
            try
            {
                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Response.Cache.SetExpires(DateTime.UtcNow.AddHours(-1));
                Response.Cache.SetNoStore();
            }catch(Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private void sendMail(string email, string token, string domainUrl)
        {
            try
            {
                MailAddress from = new MailAddress("customer_service@customer.com");
                MailAddress to = new MailAddress(email);
                MailMessage message = new MailMessage(from, to);
                message.Subject = "Reset your password";
                string htmlMessage = System.IO.File.ReadAllText(HostingEnvironment.MapPath(@"~/Views/Shared/EmailMessage.html"));
                Attachment att = new Attachment(HostingEnvironment.MapPath(@"~/Content/Images/logo.png"));
                att.ContentId = "logo.png";
                message.Attachments.Add(att);
                string url = domainUrl + "/Account/ResetPassword?token=" + token;
                htmlMessage = htmlMessage.Replace("{link}", url);
                message.Body = htmlMessage;
                message.IsBodyHtml = true;
                SmtpClient client = new SmtpClient("smtp.mailtrap.io", 465)
                {
                    Credentials = new NetworkCredential("8ed720eebbbcd2", "00d8b23e39ae8e"),
                    EnableSsl = true
                };
                client.Send(message);

            }
            catch (SmtpException ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
        private string GenerateToken(string email)
        {
            try
            {
                string mailHash = Encryptor.MD5Hash(email);
                var ticks = DateTime.Now.Ticks;
                var guid = Guid.NewGuid().ToString();
                var uniqueSessionId = ticks.ToString() + '-' + guid + '/' + mailHash;
                return uniqueSessionId;
            }catch(Exception e)
            {
                Console.WriteLine(e.ToString());
                return null;
            }

        }
        [WithoutAuthorize]
        public ActionResult ResetPassword(string token)
        {
            if (CheckCorrectTokenEmail(token) && DBOperation.CheckToken(token))
            {
                ResetPasswordViewModel u=new ResetPasswordViewModel();
                u.Token = token;
                return View(u);
            }

            TempData["msg"] = "'showExpired'";
            return RedirectToAction("LoginPage", "Account");
            
        }
        [WithoutAuthorize]
        [HttpPost]
        public ActionResult ResetPassword(ResetPasswordViewModel u)
        {
            System.Diagnostics.Debug.WriteLine(u.Token);
            if (ModelState.IsValid)
            {
                string password = Encryptor.MD5Hash(u.Password);
                DBOperation.ModifyPassword(password, u.Token);
                TempData["msg"] = "'showChangePassword'";
                return RedirectToAction("LoginPage", "Account");
            }
            return View(u);
        }
        private bool CheckCorrectTokenEmail(string token)
        {
            try
            {
                string email = DBOperation.GetEmailToken(token);
                string[] splitToken = token.Split('/');
                if (splitToken[1] == Encryptor.MD5Hash(email))
                    return true;

                return false;
            }
            catch(Exception e)
            {
                Console.WriteLine(e.ToString());
                return false;
            }
        }
        
    }
}