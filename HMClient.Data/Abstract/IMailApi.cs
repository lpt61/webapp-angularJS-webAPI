using HMClient.Data.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMClient.Data.Abstract
{
    public interface IMailApi<TApp, TDomain, TAccount, TMessage>
    {
        //TApp ServerConnect();
        //TDomain GetDomainFromAddress(string address);
        TAccount GetAccountFromAddress(string address);
        TAccount AccountLogin(string address, string password);
        void AddAccount(string address, string password);
        bool ChangePassword(string address, string newPassword);
        IEnumerable<TMessage> GetAllMessages(string address, string folder);
        TMessage GetMessage(string address, string folder, long messageId);
        bool MoveMessageToTrashBin(string address, string fromFolder, long messageId);
        TMessage UpdateMessage(string address, long messsageId, string to, string subject, string body);
        bool DeleteMessage(string address, string folder, long? messageId);
        bool SaveMessage(EmailSettings sender, string folderToSave, IEnumerable<string> toAddresses, string subject, string body);
        bool SendMessage(EmailSettings sender, IEnumerable<string> toAddresses, string subject, string body);
    }
}
