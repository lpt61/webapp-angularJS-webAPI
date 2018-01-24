using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Data.Entity.Infrastructure.Annotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using System;
using hMailServer;

namespace HMClient.Data.Models
{

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<UserSession> UserSessions { get; set; }
        //public DbSet<Draft> Drafts { get; set; }
        //public DbSet<Account> Accounts { get; set; }

        public ApplicationDbContext()
            : base("GreenMailDBContext", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        //protected override void OnModelCreating(DbModelBuilder modelBuilder)
        //{
            //base.OnModelCreating(modelBuilder);
            //modelBuilder.Entity<ApplicationUser>()
            //    .HasRequired(m => m.Account).WithRequiredPrincipal();

            //modelBuilder.Entity<ApplicationUser>().Property(au => au.AccountID).IsRequired()
            //    .HasColumnAnnotation("Index", new IndexAnnotation(new IndexAttribute("AccountID") { IsUnique = true }));

            //modelBuilder.Entity<Message>().HasKey(m => m.MessageId).ToTable("hm_messages");
            //modelBuilder.Entity<Message>().Property(m => m.MessageId).HasColumnName("messageid");
            //modelBuilder.Entity<Message>().Property(m => m.UserId).HasColumnName("messageaccountid");
            //modelBuilder.Entity<Message>().Property(m => m.MessageFileName).HasColumnName("messagefilename");
            //modelBuilder.Entity<Message>().Property(m => m.FromAddress).HasColumnName("messagefrom");
            //modelBuilder.Entity<Message>().Property(m => m.DateCreated).HasColumnName("messagecreatetime");

            //modelBuilder.Entity<Message>()
            //    .HasRequired(m => m.Users)
            //    .WithMany(a => a.Messages)
            //    .HasForeignKey(m => m.UserId);
          
            //modelBuilder.Entity<Account>().ToTable("hm_accounts");
            //modelBuilder.Entity<Account>().Property(m => m.ID).HasColumnName("accountid");
            //modelBuilder.Entity<Account>().Property(m => m.DomainID).HasColumnName("accountdomainid");
            //modelBuilder.Entity<Account>().Property(m => m.Address).HasColumnName("accountaddress");

            //modelBuilder.Entity<ApplicationUser>().HasKey(m => m.Id).ToTable("AspNetUsers");

            //modelBuilder.Entity<ApplicationUserLogin>().HasKey(ul => new { ul.LoginProvider, ul.ProviderKey, ul.UserId }).ToTable("AspNetUserLogins");

            //modelBuilder.Entity<ApplicationUserRole>().HasKey(ur => new { ur.UserId, ur.RoleId }).ToTable("AspNetUserRoles");
        //}
    }

    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public Account HMAccount { get; set; }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager, string authenticationType)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, authenticationType);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class UserSession
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string OwnerUserId { get; set; }

        public virtual ApplicationUser OwnerUser { get; set; }

        [Required]
        [MaxLength(1024)]
        public string AuthToken { get; set; }

        [Required]
        public DateTime ExpirationDateTime { get; set; }
    }

    //public class Draft
    //{
    //    [Key]
    //    public int Id { get; set; }

    //    [Required]
    //    public string OwnerUserId { get; set; }

    //    public virtual ApplicationUser OwnerUser { get; set; }

    //    [Required]
    //    public string Subject { get; set; }

    //    [Required]
    //    public string Body { get; set; }
    //}

    //public class Outbox
    //{
    //    [Key]
    //    public int Id { get; set; }

    //    [Required]
    //    public string OwnerUserId { get; set; }

    //    public virtual ApplicationUser OwnerUser { get; set; }

    //    [Required]
    //    public string To { get; set; }

    //    [Required]
    //    public string Subject { get; set; }

    //    [Required]
    //    public string Body { get; set; }

    //    [Required]
    //    public DateTime DateSent { get; set; }
    //}
}