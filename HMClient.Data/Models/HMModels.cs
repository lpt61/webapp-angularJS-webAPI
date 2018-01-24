using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Text;

namespace HMClient.Data.Models
{
    //public class HMContext : BaseDBFContext<HMContext>
    //{
    //    public DbSet<Message> Messages { get; set; }
    //    public DbSet<Account> Accounts { get; set; }

    //    protected override void OnModelCreating(DbModelBuilder modelBuilder)
    //    {
    //        //base.OnModelCreating(modelBuilder);
    //        modelBuilder.Entity<Message>().ToTable("hm_messages");
    //        modelBuilder.Entity<Message>().HasKey(m => m.MessageId);
    //        modelBuilder.Entity<Message>().Property(m => m.MessageId).HasColumnName("messageid");
    //        modelBuilder.Entity<Message>().Property(m => m.ToId).HasColumnName("messageaccountid");
    //        modelBuilder.Entity<Message>().Property(m => m.FromId).HasColumnName("messageuid");
    //        modelBuilder.Entity<Message>().Property(m => m.MessageFileName).HasColumnName("messagefilename");
    //        modelBuilder.Entity<Message>().Property(m => m.FromAddress).HasColumnName("messagefrom");
    //        modelBuilder.Entity<Message>().Property(m => m.DateCreated).HasColumnName("messagecreatetime");

    //        //modelBuilder.Entity<Message>()
    //        //    .HasRequired(m => m.Account)
    //        //    .WithMany(a => a.Messages)
    //        //    .HasForeignKey(m => m.AccountID);

    //        modelBuilder.Entity<Account>().ToTable("hm_accounts");
    //        modelBuilder.Entity<Account>().HasKey(m => m.AccountId);
    //        modelBuilder.Entity<Account>().Property(m => m.AccountId).HasColumnName("accountid");
    //        modelBuilder.Entity<Account>().Property(m => m.DomainId).HasColumnName("accountdomainid");
    //        modelBuilder.Entity<Account>().Property(m => m.AccountAddress).HasColumnName("accountaddress");
    //    }
    //}

    //[Table("hm_messages")]
    //public class Message
    //{
    //    //hm_messages.messageid, db type = bigint <=> long in C#
    //    [Column("messageid")]
    //    public long MessageId { get; set; }
        
    //    //hm_messages.messageaccountid , id of the receiver
    //    [Required]
    //    [Column("messageaccountid")]
    //    public int ToId { get; set; }

    //    //hm_messages.messageuid, id of the sender
    //    [Column("messageuid")]
    //    public int FromId { get; set; }

    //    //hm_messages.messagefilename
    //    [Column("messagefilename")]
    //    public string MessageFileName { get; set; }
    //    //hm_messages.messagefrom
    //    [Column("messagefrom")]
    //    public string FromAddress { get; set; }
    //    //hm_messages.messagecreatetime
    //    [Column("messagecreatetime")]
    //    public DateTime DateCreated { get; set; }

    //    public virtual Account Account { get; set; }
    //}

    //[Table("hm_accounts")]
    //public class Account
    //{
    //    //hm_accounts.accountid
    //    [Column("accountid")]
    //    public int AccountId { get; set; }
    //    //hm_accounts.accountdomainid
    //    [Column("accountdomainid")]
    //    public string DomainId { get; set; }
    //    //hm_accounts.accountaddress
    //    [Column("accountaddress")]
    //    public string AccountAddress { get; set; }

    //    public virtual ICollection<Message> Messages { get; set; }
    //}
}
