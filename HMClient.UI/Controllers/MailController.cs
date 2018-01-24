using HMClient.Data.Concrete;
using HMClient.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Microsoft.AspNet.Identity;
using hMailServer;
using System.Web.Http.Description;
using HMClient.UI.Models.Mails;
using HMClient.UI.Utilities;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using HMClient.Data.Abstract;
using System.Threading.Tasks;

namespace HMClient.UI.Controllers
{
    [SessionAuthorize]
    [RoutePrefix("api/mail")]
    public class MailController : BaseApiController
    {
        private ApplicationUserManager _userManager;

        public ApplicationUserManager UserManager
        {
            get
            {
                //using Microsoft.AspNet.Identity.Owin;
                return _userManager ?? Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        private EmailSettings mailBox
        {
            get; set;
        }

        public MailController(IRepository repo) : base(repo)
        {
        }

        public MailController()  : base(new EFRepository())
        {
            this._userManager = new ApplicationUserManager(new UserStore<ApplicationUser>(new ApplicationDbContext()));
        }

        //GET: api/Mail
        //public async Task<IEnumerable<Message>> Get(string folder)
        //{
        //    ApplicationUser user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
        //    return this.repository.GetMessage(user.Email, folder, null);
        //}

        public async Task<IEnumerable<MyMessage>> Get(string folder)
        {
            ApplicationUser user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            List<MyMessage> msgs = new List<MyMessage>();

            IEnumerable<Message> temp = this.repository.GetAllMessages(user.Email, folder);

            foreach (var m in temp)
            {
                msgs.Add(Helper.CreateMyMessage(m));
            }
            return msgs;
        }

        // GET: api/Mail/5
        [ResponseType(typeof(Message))]
        //'id' is the name of the key in RouteTable, if you use another name, ex: mailId, 
        //this action could not be never called 
        public async Task<IHttpActionResult> Get(string folder, long id)
        {
            ApplicationUser user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            Message mail = this.repository.GetMessage(user.Email, folder, id);
            if (mail == null)
            {
                return NotFound();
            }
            return Ok(Helper.CreateMyMessage(mail));
        }

        // PUT: api/Mail/5      (UPDATE)
        //[ResponseType(typeof(void))]
        //public IHttpActionResult PutMail(long id, Message mail)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    if (id != mail.ID)
        //    {
        //        return BadRequest();
        //    }

        //    repository.Entry(mail).State = EntityState.Modified;

        //    try
        //    {
        //        repository.SaveChanges();
        //    }
        //    catch (repositoryUpdateConcurrencyException)
        //    {
        //        if (!MailExists(id))
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return StatusCode(HttpStatusCode.NoContent);
        //}

        //PUT: api/Mail/5      (UPDATE)
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutMail(SendMailBindingModel model)
        {
            ApplicationUser user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            Message resultMsg = this.repository.UpdateMessage(user.Email, model.ID, model.To, model.Subject, model.Body);

            var result = new
            {
                resultMsg = resultMsg,
                result = resultMsg != null ? "Mail updated successfully" : "Error updating mail"
            };

            return Ok(result);
        }

        [HttpPost]
        [ResponseType(typeof(string))]
        public async Task<IHttpActionResult> Post(SendMailBindingModel model)
        {
            ApplicationUser user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            Account acc = this.repository.GetAccountFromAddress(user.Email);

            this.mailBox = new EmailSettings(
                user.Email,
                user.UserName,
                acc.Password
                //Helper.GetDomainNameFromAddress(this.user.Email)    //domain
           );

            if (model.IsDraft)
            {
                //Draft mail does not require model validations
                bool saveSucceed = this.repository.SaveMessageAsDraft(this.mailBox, new List<string>{model.To}, model.Subject, model.Body);
                return Ok(saveSucceed ? "Your mail has been saved as draft" : "Error saving mail");
            }
            else
            {
                ToModel tm = Helper.CheckEmailInputFromClient(model.To);

                if (!tm.IsValid)
                {
                    ModelState.AddModelError("To", "Invalid recipient address(es)");
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }               

                //System.Net.Mail.MailMessage mailMessage = new System.Net.Mail.MailMessage(
                //    user.Email,
                //    model.To, // To
                //    model.Subject, // Subject
                //    model.Body); // Body

                //bool IsSuccessful = this.repository.SendMessage(this.mailBox, mailMessage);

                bool sendSucceed = this.repository.SendMessage(this.mailBox, tm.Addresses, model.Subject, model.Body);

                return Ok(sendSucceed ? "Your mail has been sent" : "Error sending mail");
            }
        }

        //// DELETE: api/Mail/5
        [HttpDelete]
        [ResponseType(typeof(string))]
        public async Task<IHttpActionResult> Delete(string folder, long id)
        {
            bool isSuccessful = true;
            string result = null;
            ApplicationUser user = await UserManager.FindByIdAsync(User.Identity.GetUserId());

            if (folder.ToUpper() != "TRASHBIN")
            {
                isSuccessful = this.repository.MoveMessageToTrashBin(user.Email, folder, id);
                result = "Message moved to trash bin successfully";
            }
            else /*folder == "TRASHBIN"*/
            {
                isSuccessful = this.repository.DeleteMessage(user.Email, folder.ToUpper(), id);
                result = "Message deleted successfully";
            }

            return Ok(isSuccessful ? result : "Error happened");
        }
    }
}

////Login => AppDomainSetup current user => AppDomainSetup emailsetting;
////prepare message to send
////message.From = emailSettings.FromAddress, // From
////message.To = toAddress, // To
////message.Subject = subject, // Subject
////message.Body = body); // Body

