using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApplication1.Models
{
    public class EmailModel
    {
        public string ToEmail { get; set; }
        public string Subject { get; set; }
        [AllowHtml]
        public string Body { get; set; }
    }

}