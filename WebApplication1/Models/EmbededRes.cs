using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;

namespace WebApplication1.Models
{
    public class EmbededRes
    {
        public string htmldata { get; set; }
        public AttachmentCollection attachments { get; set; }
    }

    public class EmbededRes2
    {
        public string htmldata { get; set; }
        public AlternateView attachments { get; set; }
    }
}