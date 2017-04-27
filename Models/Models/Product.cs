using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class Product :BaseModel
    {
        [Required]
        [DataType(DataType.Currency)]
        public decimal Price { get; set; }

        [Required]

        public string ProductName { get; set; }

        public string Description { get; set; }
        public List<OrderItem> OrderItems { get; set; }
    }
}
