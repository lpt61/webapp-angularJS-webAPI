using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HMClient.UI.Models.Mails
{
    public class MailBindingModel
    {
        [Display(Name = "Date receive")]
        public DateTime DateReceive { get; set; }

        public string From { get; set; }

        public string Subject { get; set; }

        public string Body { get; set; } 
    }
}