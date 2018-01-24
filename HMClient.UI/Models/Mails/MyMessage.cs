using hMailServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HMClient.UI.Models.Mails
{
    public class MyMessage
    {
        public long ID { get; set; }
        public string From { get; set; }
        public string FromAddress { get; set; }
        public string To {get;set;}
        public string Subject { get; set; }
        public string Body { get; set; }
        public int Status { get; set; }
        public string Date { get; set; }
    }
}