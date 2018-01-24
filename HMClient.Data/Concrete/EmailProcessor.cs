using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace HMClient.Data.Concrete
{
    public class EmailSettings
    {
        //public string MailToAddress = null;
        public string FromAddress;
        //public bool UseSsl = true;

        //NetworkCredentias
        public string UserName;
        public string Password;

        public string DomainName = "greenmail.com";
        public int ServerPort = 25;

        //public string Username = "MySmtpUsername";
        //public string Password = "MySmtpPassword";
        //public string ServerName = "smtp.example.com";
        //public int ServerPort = 587;

        //public bool WriteAsFile = false;
        //public string FileLocation = @"f:\greensol_emails";

        public EmailSettings(string fromAddress, string userName, string password)
        {
            this.FromAddress = fromAddress;
            this.UserName = userName;
            this.Password = password;
            //this.DomainName = domainName;
        }
    }

    //public class EmailProcessor
    //{
    //    private EmailSettings emailSettings;

    //    public EmailProcessor(EmailSettings settings)
    //    {
    //        emailSettings = settings;
    //    }

    //    public bool SendMail(MailMessage message)
    //    {
    //        bool isSuccess = true;
    //        try
    //        {
    //            using (var smtpClient = new System.Net.Mail.SmtpClient())
    //            {
    //                //Not required
    //                //smtpClient.EnableSsl = emailSettings.UseSsl;

    //                smtpClient.Host = emailSettings.DomainName;
    //                smtpClient.Port = emailSettings.ServerPort;
    //                smtpClient.UseDefaultCredentials = false;
    //                smtpClient.Credentials = new NetworkCredential(
    //                    emailSettings.UserName,
    //                    emailSettings.Password);

    //                //Additional line
    //                //Send through a server requires Network delivery method. 
    //                smtpClient.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;

    //                //StringBuilder body = new StringBuilder()
    //                //.AppendLine("xxxxxxxxxxxxxxxxxxxxxxxxxxxx").Insert(0, "asdasdasdasdasdasdasdasd");
				
    //                smtpClient.Send(message);
    //            }
    //        }
    //        catch (Exception ex)
    //        {
    //            isSuccess = false;
    //            throw ex;
    //        }
    //        return isSuccess;
    //    }
    //}
}
