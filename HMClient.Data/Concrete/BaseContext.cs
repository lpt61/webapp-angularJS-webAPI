using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMClient.Data.Models
{
    public abstract class BaseDBFContext<TContext> : DbContext where TContext : DbContext
    {
        private const string context = "GreenMailDBContext";
        static BaseDBFContext()
        {
            Database.SetInitializer<TContext>(null);
        }

        protected BaseDBFContext() : base(context)
        {

        }
    }
}
