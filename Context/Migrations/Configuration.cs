namespace Context.Migrations
{
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using Models;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<Context.ApplicationContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(Context.ApplicationContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //
            InsertDummyData(context);
        }


        void SetUpRoles(ApplicationContext context)
        {
            var roleStore = new RoleStore<IdentityRole>(context);
            var roleManager = new RoleManager<IdentityRole>(roleStore);
            var role = new IdentityRole
            {
                Name = "Admin"
            };

            if (!context.Roles.Any(a => a.Name == "Admin")) roleManager.Create(role);


            var userStore = new UserStore<ApplicationUser>(context);
            var manager = new ApplicationUserManager(userStore);


            if (!context.Users.Any(u => u.Email == "Admin@yopmail.com"))
            {
                var user = new ApplicationUser
                {
                    Email = "Admin@yopmail.com",
                    UserName = "Admin@yopmail.com",

                };
                var result = manager.Create(user, "1234567");

                if (result.Succeeded) manager.AddToRole(user.Id, "Admin");

            }





        }


        void InsertDummyData(ApplicationContext context)
        {
            int index = 1;
            if (!context.Products.Any())
            {
                var products = new List<Product>();
                for (int i = 0; i < 20; i++)
                {
                    products.Add(new Product()
                    {
                        Description = RandomString(30),
                        Price = RandomNumberBetween(10, 100),
                        ProductName = RandomString(14),
                    });
                }

                context.Products.AddRange(products);

                context.SaveChanges();

                index = 0;
                var customers = products.Select(p => new Customer()
                {
                    FirstName = "Joe",
                    LastName = $"Customer Test {index}",
                    CompanyName = $"Test Company {index}",
                    Email = $"Joe.test{index++}@yopmail.com",
                    Phone = $"12345678{RandomNumberBetween(1, 10)}",
                    Orders = TestOrders(products),

                }).ToList();

                context.Database.CommandTimeout = 2000;
                context.Customers.AddRange(customers);
                context.SaveChanges();
            }

            var orders = context.Orders.Include(o => o.OrderItems.Select(prop => prop.Product)).ToList();

            orders.ForEach(o => o.OrderItems.ForEach(item =>
            {
                item.Quantity = index;
                item.Amount = index * item.Product.Price;
                index++;
                if (index == 10) index = 1;
            }));
            orders.ForEach(o => o.Total = o.OrderItems.Sum(i => i.Amount));
        }

        List<Order> TestOrders(List<Product> products)
        {
            var orders = products.Select(prod => new Order()
            {
                CreatedDate = DateTime.Now,
                OrderItems = products.Take(2).Select(p => new OrderItem() { Product = p }).ToList(),

            }).ToList();

            orders.ForEach(o => o.Total = o.OrderItems.Sum(p => p.Product.Price));

            return orders;
        }

        string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        decimal RandomNumberBetween(double minValue, double maxValue)
        {
            var next = random.NextDouble();

            return Convert.ToDecimal(minValue + (next * (maxValue - minValue)));
        }

        readonly Random random = new Random();
    }
}
 
