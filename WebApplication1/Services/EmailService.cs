using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;

namespace WebApplication1.Services
{
    public class EmailService
    {
        public static bool SendEmail(string email, string msg, string subject, AttachmentCollection attachments)
        {
            try
            {
                MailMessage mm = new MailMessage(ConfigurationManager.AppSettings["from"], email);
                mm.Subject = subject;
                mm.Body = msg;
                mm.IsBodyHtml = true;
                // Add attachments from the provided AttachmentCollection
                foreach (Attachment attachment in attachments)
                {
                    mm.Attachments.Add(attachment);
                }
                SmtpClient smtp = new SmtpClient();
                smtp.Host = ConfigurationManager.AppSettings["host"];
                //smtp.EnableSsl = true;
                NetworkCredential NetworkCred = new NetworkCredential();
                NetworkCred.UserName = ConfigurationManager.AppSettings["username"];
                NetworkCred.Password = ConfigurationManager.AppSettings["password"];
                smtp.UseDefaultCredentials = true;
                smtp.Credentials = NetworkCred;
                smtp.Port = Convert.ToInt32(ConfigurationManager.AppSettings["port"]);
                smtp.Send(mm);
                return true;
            }
            catch { return false; }
        }

        public static bool SendEmailWithAlternative(string email, string subject, AlternateView alternateView)
        {
            try
            {
                MailMessage mm = new MailMessage(ConfigurationManager.AppSettings["from"], email);
                mm.Subject = subject;
                //mm.Body = msg;
                mm.IsBodyHtml = true;
                mm.AlternateViews.Add(alternateView);
                // Add attachments from the provided AttachmentCollection
                //foreach (Attachment attachment in attachments)
                //{
                //    mm.Attachments.Add(attachment);
                //}
                SmtpClient smtp = new SmtpClient();
                smtp.Host = ConfigurationManager.AppSettings["host"];
                //smtp.EnableSsl = true;
                NetworkCredential NetworkCred = new NetworkCredential();
                NetworkCred.UserName = ConfigurationManager.AppSettings["username"];
                NetworkCred.Password = ConfigurationManager.AppSettings["password"];
                smtp.UseDefaultCredentials = true;
                smtp.Credentials = NetworkCred;
                smtp.Port = Convert.ToInt32(ConfigurationManager.AppSettings["port"]);
                smtp.Send(mm);
                return true;
            }
            catch { return false; }
        }
    }
}