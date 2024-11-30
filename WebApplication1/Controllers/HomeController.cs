using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Models;
using WebApplication1.Services;

public class HomeController : Controller
{
    // GET: Email form
    public ActionResult Index()
    {
        return View("EmailForm");
    }

    // POST: Send Email
    [HttpPost]
    public ActionResult SendEmail(EmailModel emailm)
    {
        try
        {
            // Update image src in the email body
            //EmbededRes embres = UpdateImageSrc(emailm.Body);
            AlternateView av = UpdateMailBody(emailm.Body);

            // Send email
            //bool issend = EmailService.SendEmail(emailm.ToEmail, embres.htmldata, emailm.Subject, embres.attachments);
            bool issend = EmailService.SendEmailWithAlternative(emailm.ToEmail,  emailm.Subject, av);
            if (issend)
                return Content("Email sent successfully.");
            return Content("Email not sent.");
        }
        catch (Exception ex)
        {
            return Content($"Error: {ex.Message}");
        }
    }

    // POST: Upload Image
    [HttpPost]
    public JsonResult UploadImage(HttpPostedFileBase file)
    {
        try
        {
            if (file != null && file.ContentLength > 0)
            {
                // Generate a unique filename using GUID
                string uniqueFileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
                // Define the path to save the uploaded file
                string uploadPath = Server.MapPath("~/UploadedImages/");
                // Ensure the directory exists
                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                }
                // Combine the path and filename
                string fullPath = Path.Combine(uploadPath, uniqueFileName);
                // Save the file
                file.SaveAs(fullPath);
                // Return the file's new location as a response (used by TinyMCE)
                string fileUrl = Url.Content($"~/UploadedImages/{uniqueFileName}");
                return Json(new { location = fileUrl });
            }
            else
            {
                return Json(new { error = "No file uploaded" });
            }
        }
        catch (Exception ex)
        {
            return Json(new
            {
                error = ex.Message
            });
        }
    }

    // Helper method to update image src
    private EmbededRes UpdateImageSrc(string htmlBody)
    {
        EmbededRes res = new EmbededRes();
        var doc = new HtmlAgilityPack.HtmlDocument();
        doc.LoadHtml(htmlBody);
        AttachmentCollection attachments = new MailMessage().Attachments;
        foreach (var img in doc.DocumentNode.SelectNodes("//img"))
        {
            string src = img.GetAttributeValue("src", "");
            if (src.Contains("UploadedImages"))
            {
                // Get the unique file name without extension
                string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(src);
                string newSrc = $"cid:{fileNameWithoutExtension}";
                img.SetAttributeValue("src", newSrc);
                string imgpath = Server.MapPath($"~/UploadedImages/{Path.GetFileName(src)}");
                Attachment inlineLogo = new Attachment(imgpath);
                attachments.Add(inlineLogo);
                inlineLogo.ContentId = fileNameWithoutExtension;
                //To make the image display as inline and not as attachment
                inlineLogo.ContentDisposition.Inline = true;
                inlineLogo.ContentDisposition.DispositionType = DispositionTypeNames.Inline;
            }
        }
        res.htmldata = doc.DocumentNode.OuterHtml;
        res.attachments = attachments;
        return res;
    }

    private AlternateView UpdateMailBody(string htmlBody)
    {
        var doc = new HtmlAgilityPack.HtmlDocument();
        doc.LoadHtml(htmlBody);
        List<LinkedResource> linkedResources = new List<LinkedResource>();
        foreach (var img in doc.DocumentNode.SelectNodes("//img"))
        {
            string src = img.GetAttributeValue("src", "");
            if (src.Contains("UploadedImages"))
            {
                // Get the unique file name without extension
                string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(src);
                string newSrc = $"cid:{fileNameWithoutExtension}";
                img.SetAttributeValue("src", newSrc);
                string imgpath = Server.MapPath($"~/UploadedImages/{Path.GetFileName(src)}");
                LinkedResource Img = new LinkedResource(imgpath, MimeMapping.GetMimeMapping(imgpath));
                Img.ContentId = fileNameWithoutExtension;
                Img.ContentType.Name = Img.ContentId;
                Img.ContentLink = new Uri("cid:" + fileNameWithoutExtension);
                Img.TransferEncoding = TransferEncoding.Base64;
                Img.ContentType.MediaType = MimeMapping.GetMimeMapping(imgpath);
                linkedResources.Add(Img);
            }
        }
        // Create an AlternateView for HTML
        AlternateView avHtml = AlternateView.CreateAlternateViewFromString(doc.DocumentNode.OuterHtml, Encoding.UTF8, MediaTypeNames.Text.Html);
        // Add all LinkedResources to the AlternateView
        foreach (var resource in linkedResources)
        {
            avHtml.LinkedResources.Add(resource);
        }
        return avHtml;
    }

    public ActionResult GetImage(string fileName)
    {
        string filePath = Server.MapPath($"~/UploadedImages/{fileName}");
        return File(filePath, "image/jpeg");
    }
}
