using Context.Config;

using Microsoft.AspNet.Identity.EntityFramework;
using Models;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;


namespace Context
{
     
    public class ApplicationContext : IdentityDbContext<ApplicationUser>
    {

        public ApplicationContext(): base("OracleDbContext")
        {
            this.Configuration.LazyLoadingEnabled = false;
        }
        public DbSet<IdentityUserRole> UserRoles { get; set; }

        public DbSet<IdentityUserClaim> UserClaims { get; set; }


        public  DbSet<Order> Orders { get; set; }

        public DbSet<Customer> Customers { get; set; }

        public DbSet<OrderItem> OrderItems { get; set; }

        
        public DbSet<Product> Products { get; set; }

    

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.HasDefaultSchema("C##USR_PATRONATO");

            modelBuilder.Conventions.Remove<PluralizingEntitySetNameConvention>();
            modelBuilder.Configurations.Add(new CustomerConfig());
            modelBuilder.Configurations.Add(new OrderItemConfig());
            modelBuilder.Configurations.Add(new OrderConfig());
          
        }


        public static ApplicationContext Create()
        {
            return new ApplicationContext();
        }

       
        
    }
}
