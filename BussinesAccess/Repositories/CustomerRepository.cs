using Context;
using Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BussinesAccess.Repositories
{
    public class CustomerRepository : BaseRepository<ApplicationContext, Customer>
    {
        /// <summary>
        /// returns customers list 20 by default
        /// </summary>
        /// <param name="keywords"></param>
        /// <returns></returns>
        public Task<List<Customer>> Search(string keywords)
        {
            var keys = (keywords ?? "").ToLower().Trim().Split(' ').ToList();

            IQueryable<Customer> query = Context.Customers.OrderBy(o=>o.Id);

            // simple search engine
            keys.ForEach(k => query = query.Where(c => c.FirstName.Contains(k) || c.LastName.Contains(k) || c.Email.Contains(k)));

            return query.Take(20).ToListAsync();

        }

        public Task<List<Order>> GetOrders(int id)
        {
            return Context.Orders.Where(o => o.CustomerId == id).ToListAsync();
        }
    }
}
