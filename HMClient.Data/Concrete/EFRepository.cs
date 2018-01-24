using hMailServer;
using HMClient.Data.Abstract;
using HMClient.Data.Concrete;
using HMClient.Data.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace HMClient.Data.Concrete
{
    public class EFRepository : IRepository
    {
        private IMailApi<ApplicationClass, Domain, Account, Message> api = new HMSApi();

        private ApplicationDbContext identityContext = new ApplicationDbContext();

        public IEnumerable<ApplicationUser> ApplicationUsers
        {
            get { return identityContext.Users; }
        }

        public IEnumerable<UserSession> UserSessions
        {
            get { return identityContext.UserSessions; }
        }

        #region USER

        //This method is only for supporting EditUserProfile()
        public ApplicationUser GetUserByUserID(string currentUserID)
        {
            var currentUser = identityContext.Users.First(u => u.Id == currentUserID);

            return currentUser;
        }

        public Account GetAccountFromAddress(string address)
        {
            return this.api.GetAccountFromAddress(address);
        }

        public int EditUserProfile(string currentUserID, string userName, string email)
        {
            ApplicationUser currentUser = GetUserByUserID(currentUserID);
            if (currentUser == null)
            {
                return 0;
            }

            var hasEmailTaken = identityContext.Users.Any(x => x.Email == email);
            if (hasEmailTaken)
            {
                return 3;
            }

            var nameHasTaken = identityContext.Users.Any(x => x.UserName == userName);
            if (nameHasTaken)
            {
                return 2;
            }

            currentUser.UserName = userName;
            currentUser.Email = email;

            identityContext.SaveChanges();

            return 1;
        }

        public bool ChangePassword(string address, string newPassword)
        {
            return api.ChangePassword(address, newPassword);
        }

        public void AddAccount(string email, string password)
        {            
            api.AddAccount(email, password);
        }

        #endregion USER

        #region SESSION

        /// <summary>
        /// Extends the validity period of the current user's session in the database.
        /// This will configure the user's bearer authorization token to expire after
        /// certain period of time (e.g. 30 minutes, see UserSessionTimeout in Web.config)
        /// </summary>
        public void AddUserSession(string username, string authToken, TimeSpan duration)
        {
            var userId = this.identityContext.Users.First(u => u.UserName == username).Id;
            var userSession = new UserSession()
            {
                OwnerUserId = userId,
                AuthToken = authToken
            };
            this.identityContext.UserSessions.Add(userSession);

            // Extend the lifetime of the current user's session: current moment + fixed timeout
            userSession.ExpirationDateTime = DateTime.Now + duration;
            this.identityContext.SaveChanges();
        }

        /// <summary>
        /// Makes the current user session invalid (deletes the session token from the user sessions).
        /// The goal is to revoke any further access with the same authorization bearer token.
        /// Typically this method is called at "logout".
        /// </summary>
        public void DeleteUserSession(string authToken, string currentUserID)
        {
            UserSession userSession = userSession = this.identityContext.UserSessions.FirstOrDefault(
                session => session.AuthToken == authToken
                && session.OwnerUserId == currentUserID);         

            if (userSession != null)
            {
                this.identityContext.UserSessions.Remove(userSession);
                this.identityContext.SaveChanges();
            }                       
        }

        public void DeleteExpiredSessions()
        {
            var userSessions = this.identityContext.UserSessions.Where(
                session => session.ExpirationDateTime < DateTime.Now);

            this.identityContext.UserSessions.RemoveRange(userSessions);
        }

        // Summary:
        // Re-validates the user session. Usually called at each authorization request.
        // If the session is not expired, extends it lifetime and returns true.
        // If the session is expired or does not exist, return false.
        // <returns>true if the session is valid</returns>
        public bool UpdateUserSession(string authToken, string currentUserID, TimeSpan duration)
        {
            var userSession = this.identityContext.UserSessions.First(session =>
                session.AuthToken == authToken 
                && session.OwnerUserId == currentUserID);

            if (userSession == null)
            {
                // User does not have a session with this token --> invalid session
                return false;
            }

            if (userSession.ExpirationDateTime < DateTime.Now)
            {
                // User's session is expired --> invalid session
                return false;
            }

            // Extend the lifetime of the current user's session: current moment + fixed timeout
            userSession.ExpirationDateTime = DateTime.Now + duration;
            this.identityContext.SaveChanges();

            return true;
        }
        
        #endregion SESSION

        #region MESSAGE

        public bool SaveMessageAsDraft(EmailSettings sender, IEnumerable<string> toAddresses, string subject, string body)
        {
            return this.api.SaveMessage(sender, "DRAFT", toAddresses, subject, body);
        }

        public bool SendMessage(EmailSettings sender, IEnumerable<string> to, string subject, string body)
        {
            //HMServer API also provide a method to send messages

            //Use .NET SMTP to send messages
            //EmailProcessor eP = new EmailProcessor(settings);
            //return eP.SendMail(message);

            //Use hMailServer API to send mail
            return this.api.SendMessage(sender, to, subject, body);
         
        }

        public IEnumerable<Message> GetAllMessages(string address, string folder)
        {
            return this.api.GetAllMessages(address, folder);
        }

        public Message GetMessage(string address, string folder, long messageId)
        {
            return this.api.GetMessage(address, folder, messageId);
        }

        public Message UpdateMessage(string address, long messsageId, string to, string subject, string body)
        {
            return this.api.UpdateMessage(address, messsageId, to, subject, body);
        }

        public bool MoveMessageToTrashBin(string address, string fromFolder, long messageId)
        {
            return this.api.MoveMessageToTrashBin(address, fromFolder, messageId);
        }

        public bool DeleteMessage(string address, string folder, long? messageId)
        {
            return this.api.DeleteMessage(address, folder, messageId);
        }

        #region OBSOLETE
        /*These functions are replaced by API functions*/
        //private HMContext hMContext = new HMContext();

        //public IEnumerable<Account> Accounts
        //{
        //    get
        //    {
        //        return hMContext.Accounts;
        //    }
        //}

        //public IEnumerable<Message> Messages
        //{
        //    get
        //    {
        //        return hMContext.Messages;
        //    }
        //}

        //public IEnumerable<UserSession> UserSessions
        //{
        //    get
        //    {
        //        return identityContext.UserSessions;
        //    }
        //}

        //public Account GetAccountByAccountID(int accountID)
        //{
        //    return hMContext.Accounts.Single(m => m.AccountId == accountID);
        //}

        //public Message GetMessageByMessageID(int messageID)
        //{
        //    return hMContext.Messages.Single(m => m.MessageId == messageID);
        //}

        //public IEnumerable<Message> GetMessagesByAccountID(int accountID)
        //{
        //    return hMContext.Messages.Where(m => m.ToId == accountID);
        //}

        //public int DeleteMessage(int messageID)
        //{
        //    int success = 1;

        //    Message msg = hMContext.Messages.Single(m => m.MessageId == messageID);

        //    if (msg == null)
        //    {
        //        success = 0;
        //    }
        //    else
        //    {
        //        hMContext.Messages.Remove(msg);
        //        hMContext.SaveChanges();
        //    }

        //    return success;
        //}
        #endregion
        
        #endregion MESSAGE

        public void Dispose()
        {
            identityContext.Dispose();
        }
    }
}


