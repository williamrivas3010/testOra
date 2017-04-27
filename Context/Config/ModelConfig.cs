using Models;
using System.Data.Entity.ModelConfiguration;

/// <summary>
///  in this file we define all tables relations with another tables using Fluent Api 
/// </summary>
namespace Context.Config
{

    public class CustomerConfig : EntityTypeConfiguration<Customer>
    {
        public CustomerConfig()
        {

            HasOptional(p => p.ApplicationUser).WithRequired(p => p.Customer);
        }
    }


    public class OrderItemConfig : EntityTypeConfiguration<OrderItem>
    {
        public OrderItemConfig()
        {
            HasRequired(p => p.Order).WithMany(p => p.OrderItems).HasForeignKey(p => p.OrderId).WillCascadeOnDelete(true);
            HasRequired(p => p.Product).WithMany(p => p.OrderItems).HasForeignKey(p => p.ProductId).WillCascadeOnDelete(false);
        }
    }

    public class OrderConfig : EntityTypeConfiguration<Order>
    {
        public OrderConfig()
        {
            HasRequired(p => p.Customer).WithMany(p => p.Orders).HasForeignKey(p => p.CustomerId).WillCascadeOnDelete(false);
        }
    }

}                                                                                      
