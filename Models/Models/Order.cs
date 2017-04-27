using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Models
{
    public  class Order:BaseModel
    {

        public DateTime CreatedDate { get; set; }


        public int CustomerId { get; set; }

        
        public Customer Customer { get; set; }


        [DataType(DataType.Currency)]
        public Decimal Total { get; set; }
        public List<OrderItem> OrderItems { get; set; }
    }
}
