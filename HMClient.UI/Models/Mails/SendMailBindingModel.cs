using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HMClient.UI.Models.Mails
{
    public class SendMailBindingModel
    {
        public long ID { get; set; } 
        [Required]
        public string To { get; set; }
        [Required(ErrorMessage = "Subject is required")]
        public string Subject { get; set; }
        [Required(ErrorMessage = "Body is required")]
        public string Body { get; set; }
        public bool IsDraft { get; set; }
    }
}