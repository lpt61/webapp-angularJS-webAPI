using hMailServer;
using HMClient.Data.Concrete;
using HMClient.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace HMClient.Data.Abstract
{
    public interface IRepository
    {
        //IEnumerable<Message> Messages { get; }

        IEnumerable<ApplicationUser> ApplicationUsers { get; }

        IEnumerable<UserSession> UserSessions { get; }

        ApplicationUser GetUserByUserID(string currentUserID);

        Account GetAccountFromAddress(string address);

        void AddAccount(string email, string password);

        void AddUserSession(string username, string authToken, TimeSpan duration);

        void DeleteUserSession(string authToken, string currentUserID);

        void DeleteExpiredSessions();

        bool UpdateUserSession(string authToken, string currentUserID, TimeSpan duration);

        int EditUserProfile(string currentUserID, string userName, string email);

        bool ChangePassword(string address, string newPassword);

        IEnumerable<Message> GetAllMessages(string address, string folder);

        Message GetMessage(string address, string folder, long messageId);

        Message UpdateMessage(string address, long messsageId, string to, string subject, string body);

        bool MoveMessageToTrashBin(string address, string fromFolder, long messageId);

        bool DeleteMessage(string address, string folder, long? messageId);

        bool SaveMessageAsDraft(EmailSettings sender, IEnumerable<string> toAddresses, string subject, string body);

        bool SendMessage(EmailSettings settings, IEnumerable<string> to, string subject, string body);

        void Dispose();
    }
}
