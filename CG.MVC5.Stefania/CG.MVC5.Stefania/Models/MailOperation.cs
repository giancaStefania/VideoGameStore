using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Hosting;

namespace CG.MVC5.Stefania.Models
{
    public class MailOperation
    {
        public static void SendMailOrder(string mail_address, string subject, string html_message_file_name, int number_order,string domainUrl)
        {
            try
            {
                MailAddress from = new MailAddress("customer_service@customer.com");
                MailAddress to = new MailAddress(mail_address);
                MailMessage message = new MailMessage(from, to);
                message.Subject = subject;
                string htmlMessage = System.IO.File.ReadAllText(HostingEnvironment.MapPath(@"~/Views/Shared/"+html_message_file_name));
                Attachment att = new Attachment(HostingEnvironment.MapPath(@"~/Content/Images/logo.png"));
                att.ContentId = "logo.png";
                message.Attachments.Add(att);
                htmlMessage = htmlMessage.Replace("{number}", number_order.ToString());
                string url = domainUrl + "/Order";
                htmlMessage = htmlMessage.Replace("{link}", url);
                message.Body = htmlMessage;
                message.IsBodyHtml = true;
                SmtpClient client = new SmtpClient("smtp.mailtrap.io", 465)
                {
                    Credentials = new System.Net.NetworkCredential("8ed720eebbbcd2", "00d8b23e39ae8e"),
                    EnableSsl = true
                };
                client.Send(message);

            }
            catch (SmtpException ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
        public static void SendMailProductDeleted(string mail_address, string subject, string html_message_file_name, int number_order, ProductE product, string domainUrl)
        {
            try
            {
                MailAddress from = new MailAddress("customer_service@customer.com");
                MailAddress to = new MailAddress(mail_address);
                MailMessage message = new MailMessage(from, to);
                message.Subject = subject;
                string htmlMessage = System.IO.File.ReadAllText(HostingEnvironment.MapPath(@"~/Views/Shared/" + html_message_file_name));
                Attachment att = new Attachment(HostingEnvironment.MapPath(@"~/Content/Images/logo.png"));
                att.ContentId = "logo.png";
                message.Attachments.Add(att);
                Attachment image = new Attachment(HostingEnvironment.MapPath(@"~/Content/Images/" + product.Id + "/" + product.Image1_string));
                image.ContentId = "image";
                message.Attachments.Add(image);
                htmlMessage = htmlMessage.Replace("{number}", number_order.ToString());
                string url = domainUrl + "/Order";
                htmlMessage = htmlMessage.Replace("{link}", url);
                htmlMessage = htmlMessage.Replace("{name}", product.Name);
                message.Body = htmlMessage;
                message.IsBodyHtml = true;
                SmtpClient client = new SmtpClient("smtp.mailtrap.io", 465)
                {
                    Credentials = new System.Net.NetworkCredential("8ed720eebbbcd2", "00d8b23e39ae8e"),
                    EnableSsl = true
                };
                client.Send(message);

            }
            catch (SmtpException ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}