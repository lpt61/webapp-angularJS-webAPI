using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using HMClient.Data.Models;
using HMClient.Data.Abstract;
using HMClient.Data.Concrete;

namespace HMClient.UI.Controllers
{
    public class HomeController : Controller
    {
        IRepository repo = new EFRepository();

        public ActionResult Index()
        {
            //var result = repo.GetMessage("user2@mymail.com", null);
            //foreach (var msg in result)
            //{
            //    System.Diagnostics.Debug.WriteLine("Date: " + msg.State);
            //    System.Diagnostics.Debug.WriteLine("Date: " + msg.Date);
            //    System.Diagnostics.Debug.WriteLine("Internal date: " + msg.InternalDate);
            //    System.Diagnostics.Debug.WriteLine("From: " + msg.From);
            //    System.Diagnostics.Debug.WriteLine("Subject: " + msg.Subject);
            //    System.Diagnostics.Debug.WriteLine("Body: " + msg.Body);
            //}


            //ApplicationDbContext context = new ApplicationDbContext();
            //var result = context.Users.Where(a => a.Email == "user1@mymail.com");
            //foreach(var item in result){
            //    System.Diagnostics.Debug.WriteLine(item.Email);
            //}
            //return View(result);

            return View();
        }
    }
}
