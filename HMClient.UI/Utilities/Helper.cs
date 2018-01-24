using hMailServer;
using HMClient.UI.Models.Mails;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HMClient.Data.Concrete
{
    public class ToModel
    {
        public bool IsValid { get; set; }
        public List<string> Addresses { get; set; } 
    }
    public static class Helper
    {
        /// <summary>
        ///  Create a custom message from HMailServer message, whose has To addresses more readable
        /// </summary>
        public static MyMessage CreateMyMessage(Message hmMessage){
            MyMessage msg = new MyMessage
            {
                ID = hmMessage.ID,
                From = hmMessage.From,
                FromAddress = hmMessage.FromAddress,
                //Manipulate HMailserver's To property to make it more readable
                To = Helper.GetEmailStringFromServer(hmMessage.To),
                Subject = hmMessage.Subject,
                Body = hmMessage.Body,
                Status = hmMessage.State,
                Date = hmMessage.Date
            };
            return msg;
        }

        public static string GetDomainNameFromAddress(string address)
        {
            //return address.Substring(address.IndexOf('@') + 1);
            return "mymail.com";
        }

        private static bool IsEmailAddress(string input)
        {
            return Regex.Match(input,"^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$").Success;
        }

        public static string GetUserNameFromAddress(string address)
        {
            int lengthOfUserName = address.Length - (1 + GetDomainNameFromAddress(address).Length);
            return address.Substring(0, lengthOfUserName);
        }

        //This helper extracts email address from HMailserver's Message.To property.
        //HMailserver Message.To has this format: "AccountName" <AccountAddress@domain.com>
        //public static string GetEmailAddress(string input)
        //{
        //    /*Input samples:
        //        "" <user2@greenmail.com>  ("\"" + " \"" + " <user2@greenmail.com>")
        //        " " <user2@greenmail.com>" ("\"" + " \"" + " <user2@greenmail.com>")
        //        "user2" <user2@greenmail.com>" ("\"" + "use r2 r3" + "\"" + " <user2@greenmail.com>")
        //        "user 1 2 3" <user2@greenmail.com>" ("\"" + "use r2 r3" + "\"" + " <user2@greenmail.com>")		
        //    */

        //    //Get AccountName and email address by eliminating characters : ", <, >
        //    //Then trim each string result to make sure the result does not contain empty string "" 
        //    //or a string that has every characters are white spaces 
        //    IEnumerable<string> strArr = Regex.Split(input, "[\"<>]").Where(s => s.Trim(null) != string.Empty);
            
        //    if (strArr.Count() == 1)
        //        return strArr.ElementAt(0);

        //    return strArr.ElementAt(1);
        //}

        public static ToModel CheckEmailInputFromClient(string input)
        {
            ToModel toModel = new ToModel { 
                IsValid = false, 
                Addresses = new List<string>()
            };

            foreach (var a in GetEmailListFromClient(input))
            {
                if (!IsEmailAddress(a))
                {
                    toModel.IsValid = false;
                    return toModel;
                }
                else
                {
                    toModel.IsValid = true;
                    toModel.Addresses.Add(a);
                }
            }
            return toModel;
        }

        private static IEnumerable<string> GetEmailListFromClient(string input)
        {
            return input.Split(null).Where(s => s.Trim(null) != string.Empty);
        }

        public static string GetEmailStringFromServer(string input)
        {
            IEnumerable<string> accsArr = Regex.Split(input, "[,]").Where(s => s.Trim(null) != string.Empty);

            IEnumerable<string> temp = null;

            string addressArr = null;
            foreach (var a in accsArr)
            {
                temp = Regex.Split(a, "[\"<>,]").Where(s => s.Trim(null) != string.Empty);
                Console.WriteLine("Temp:");
                Console.WriteLine(temp);
                if (temp.Count() == 1)
                    addressArr += temp.ElementAt(0) + " ";
                else
                    addressArr += temp.ElementAt(1) + " ";
            }

            return addressArr;
        }

        //public static IEnumerable<string> GetEmailList(string input)
        //{
        //    /*Input samples:
        //        "" <user2@greenmail.com>  ("\"" + " \"" + " <user2@greenmail.com>")
        //        " " <user2@greenmail.com>" ("\"" + " \"" + " <user2@greenmail.com>")
        //        "user2" <user2@greenmail.com>" ("\"" + "use r2 r3" + "\"" + " <user2@greenmail.com>")
        //        "user 1 2 3" <user2@greenmail.com>" ("\"" + "use r2 r3" + "\"" + " <user2@greenmail.com>")		
        //    */

        //    //IEnumerable<string> strArr = Regex.Split(input, "[\"<>,]").Where(s => s.Trim(null) != string.Empty);
        //    IEnumerable<string> accsArr = Regex.Split(input, "[,]").Where(s => s.Trim(null) != string.Empty);
        //    List<string> addressArr = new List<string>();
        //    IEnumerable<string> temp = null;
        //    foreach (var a in accsArr)
        //    {
        //        temp = Regex.Split(a, "[\"<>,]").Where(s => s.Trim(null) != string.Empty);
        //        Console.WriteLine("Temp:");
        //        Console.WriteLine(temp);
        //        if (temp.Count() == 1)
        //            addressArr.Add(temp.ElementAt(0));
        //        else
        //            addressArr.Add(temp.ElementAt(1));
        //    }
        //    return addressArr;
        //}
    }
}
