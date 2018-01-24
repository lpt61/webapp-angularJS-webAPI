using hMailServer;
using HMClient.Data.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMClient.Data.Concrete
{
    //Getting, saving, deleting messages could be done by setting message's flags instead of using IMAP folders.
    //However, after modifying message's flags, server needs to be reloaded (by calling Reinitialize()) 
    //to update changes. 
    //Reloading causes a very long delay at client side. So I use IMAP folders. 
    public class HMSApi : IMailApi<ApplicationClass, Domain, Account, Message>
    {
        private string adminName;
        private string adminPassword;

        private ApplicationClass server; 

        public HMSApi()
        {
            this.adminName = "Administrator";
            this.adminPassword = "0601344976";
            server = ServerConnect();
        }

        public HMSApi(string adminName, string adminPassword)
        {
            this.adminName = adminName;
            this.adminPassword = adminPassword;
            ServerConnect();
        }

        private ApplicationClass ServerConnect()
        {
            ApplicationClass globalObj = new ApplicationClass();
            //Credential of the admin when you setup hMailServer Administrator
            globalObj.Authenticate(this.adminName, this.adminPassword);

            return globalObj;
        }

        /// <summary>
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        private Domain GetDomainFromAddress(string address)
        {
            string domainName = address.Substring(address.IndexOf('@') + 1);
            try
            {
                return this.server.Domains.get_ItemByName(domainName);
            }
            catch
            {
                return null;
            }
        }

        public Account GetAccountFromAddress(string address)
        {
            Domain domain = this.GetDomainFromAddress(address);

            Account account =  domain.Accounts.get_ItemByAddress(address);

            if (domain != null && account != null)
                return account;
            return null;
        }

        /// <summary>
        /// Login. The domain in the address should already exist.
        /// </summary>
        /// <param name="address"></param>
        /// <param name="Password"></param>
        /// <returns></returns>
        public Account AccountLogin(string address, string password)
        {
            Domain domain = this.GetDomainFromAddress(address);
            
            Account account = domain.Accounts.get_ItemByAddress(address);
            if (account != null && account.ValidatePassword(password))
                return account;
            return null;
        }

        public bool ChangePassword(string address, string newPassword)
        {
            Account account = this.GetAccountFromAddress(address);
            try
            {
                account.Password = newPassword;
                account.Save();
            }
            catch
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Add an account to the specified domain
        /// </summary>
        /// <param name="DomainName"></param>
        /// <param name="email"></param>
        /// <param name="Password"></param>
        /// <returns></returns>
        public void AddAccount(string address, string password)
        {
            try
            {
                Domain domain = this.GetDomainFromAddress(address);
                Accounts accounts = domain.Accounts;
                Account mailbox = accounts.Add();
                
                /*Default initialize properties for an account*/
                mailbox.Address = address;
                mailbox.Password = password;
                mailbox.Active = true;
                mailbox.MaxSize = 50;
                mailbox.Save();

                //Create additional folders (INBOX is initially created by hMailServer)
                mailbox.IMAPFolders.Add("OUTBOX");
                mailbox.IMAPFolders.Add("DRAFT");
                mailbox.IMAPFolders.Add("TRASHBIN");
                mailbox.IMAPFolders.Add("READ");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //Since I manage messages by IMAP folders, I specify a folder name to get messages in it.
        public IEnumerable<Message> GetAllMessages(string address, string folder)
        {
            Account account = this.GetAccountFromAddress(address);
            IMAPFolder iFolder = account.IMAPFolders.get_ItemByName(folder.ToUpper());
            Messages msgs = iFolder.Messages;

            List<Message> result = new List<Message>();
            try
            {
                for (int i = 0; i < msgs.Count; i++)
                {
                    if (msgs[i].get_Flag(eMessageFlag.eMFDeleted) == false)
                    result.Add(msgs[i]);
                }
                
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }

        /// <summary>
        /// HMailserver gives each message a unique id no matter which folder (INBOX, OUTBOX,..) contains it. 
        /// So it's not necessary to specify IMAP folder.
        /// </summary>
        /// <param name="account"></param>
        /// <param name="folder"></param>
        /// <param name="messageId"></param>
        /// <returns></returns>
        public Message GetMessage(string address, string folder, long messageId)
        {
            Account account = this.GetAccountFromAddress(address);
            IMAPFolder iFolder = account.IMAPFolders.get_ItemByName(folder.ToUpper());
            Messages msgs = iFolder.Messages;

            List<Message> result = new List<Message>();
            try
            {
                return msgs.get_ItemByDBID((long)messageId);              
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
       
        public bool SaveMessage(EmailSettings sender, string folderToSave, IEnumerable<string> toAddresses, string subject, string body)
        {
            bool saveSucceed = true;

            //Since this message is stored in the sender account, it will not be sent, 
            //so I didn't specify outboxMsg.FromAddress to prevent hmServer from sending it
            Account senderAcc = GetAccountFromAddress(sender.FromAddress);
            IMAPFolder iFolder = senderAcc.IMAPFolders.get_ItemByName(folderToSave);
            Message msg = iFolder.Messages.Add();
            msg.From = sender.FromAddress;
            msg.Subject = subject;
            foreach (var i in toAddresses)
                msg.AddRecipient("", i);
            msg.Body = body;

            try
            {
                //Save the message 
                msg.Save();
            }
            catch (Exception ex)
            {
                saveSucceed = false;
                throw ex;
            }
            return saveSucceed;
        }

        /// <summary>
        /// Save the message to sender's OUTBOX before sending. If saving is not successful, stop sending
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="to"></param>
        /// <param name="subject"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        public bool SendMessage(EmailSettings sender, IEnumerable<string> toAddresses, string subject, string body)
        {
            //Save the message to folder OUTBOX before sending
            bool saveSucceed = SaveMessage(sender, "OUTBOX", toAddresses, subject, body);
            bool sendSucceed = true;           

            if (saveSucceed)
            {
                //This is the message which is going to be sent, I have to provide all neccessary properties for it 
                //so that hmailServer can deliver it properly. 
                Message msg = new Message();
                msg.From = sender.UserName;
                msg.FromAddress = sender.FromAddress;   //
                msg.Subject = subject;
                foreach (var i in toAddresses)
                    msg.AddRecipient("", i);
                msg.Body = body;

                try
                {
                    //Saves the email. If this is a new email, it will be delivered after saving.
                    msg.Save();
                }
                catch (Exception ex)
                {
                    sendSucceed = false;
                    throw ex;
                }
            }
            else
                sendSucceed = false;
            return sendSucceed;
        }

        public Message UpdateMessage(string address, long messsageId, string to, string subject, string body)
        {           
            Message msgToUpdate = null;
            try
            {
                Account account = this.GetAccountFromAddress(address);
                msgToUpdate = account.Messages.get_ItemByDBID(messsageId);
                msgToUpdate.ClearRecipients();
                msgToUpdate.AddRecipient("", to);
                msgToUpdate.Subject = subject;
                msgToUpdate.Body = body;
                msgToUpdate.Save();
                return msgToUpdate;
            }
            catch (Exception ex)
            {
                return null;
                throw ex;
            }         
        }

        public bool MoveMessageToTrashBin(string address,string fromFolderName, long messageId)
        {
            bool isSucceed = true;
            try
            {
                Account account = this.GetAccountFromAddress(address);
                IMAPFolder trashFolder = account.IMAPFolders.get_ItemByName("TRASHBIN");
                IMAPFolder fromFolder = account.IMAPFolders.get_ItemByName(fromFolderName);
                Message msg = fromFolder.Messages.get_ItemByDBID(messageId);

                msg.Copy(trashFolder.ID);
                fromFolder.Messages.DeleteByDBID(msg.ID);
                account.Save();
            }
            catch (Exception ex)
            {
                isSucceed = false;
                throw ex;
            }
            return isSucceed;
        }

        /// <summary>
        /// Delete a message in an IMAP folder by message Id, 
        /// if Id is not provided, delete all messages in that folder
        /// HMailserver gives each message a unique id no matter which folder (INBOX, OUTBOX,..) contains it. 
        /// So it's not necessary to specify IMAP folder 
        /// However, if I dont specify an IMAP folder, I have to call account.DeleteMessages() 
        /// and that method will delete the IMAP folders (not what I want) and messages inside them.
        /// </summary>
        /// <param name="address"></param>
        /// <param name="folder"></param>
        /// <param name="messageId"></param>
        /// <returns></returns>
        public bool DeleteMessage(string address, string folder, long? messageId)
        {
            try
            {
                Account account = this.GetAccountFromAddress(address);
                Messages msgs = account.IMAPFolders.get_ItemByName(folder).Messages;

                if (messageId != null)
                {
                    msgs.DeleteByDBID((long)messageId);
                }
                else
                {
                    //Delete all IMAP folders and messages inside each of them
                    //account.DeleteMessages();

                    msgs.Clear();
                }
                account.Save();
            }
            catch
            {
                return false;
            }
            return true;
        }

        //Enumerate IMAP folders
        public List<string> ListFolder(IMAPFolders folders, int iRecursion = 0)
        {
            iRecursion += 1;
            List<string> folderNames = new List<string>();
            for (int i = 0; i <= folders.Count - 1; i++)
            {
                folderNames.Add(folders[i].Name + iRecursion * 3);
                folderNames.AddRange(ListFolder(folders[i].SubFolders, iRecursion));
            }
            iRecursion -= 1;

            foreach (var i in folderNames)
            {
                Console.WriteLine(i);
            }

            return folderNames;
        }

        //Enumerate messages in aN IMAP folder
        public List<string> ListFiles(IMAPFolders folders, int iRecursion = 0)
        {
            iRecursion += 1;
            List<string> filenames = new List<string>();
            for (int i = 0; i <= folders.Count - 1; i++)
            {
                if (folders[i].Messages.Count == 0)
                    Console.WriteLine(folders[i].Name + " 0 messages");
                for (int y = 0; y < folders[i].Messages.Count; y++)
                {
                    Console.WriteLine(folders[i].Name + " " + folders[i].Messages[y].Filename);
                    filenames.Add(folders[i].Messages[y].Filename);
                    filenames.AddRange(ListFolder(folders[i].SubFolders, iRecursion));
                }
                Console.WriteLine("\n");
            }
            iRecursion -= 1;

            return filenames;
        }
    }
}
