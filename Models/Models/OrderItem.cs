using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class OrderItem:BaseModel
    {
        public int ProductId { get; set; }

        public int OrderId { get; set; }

        public int Quantity { get; set; }

        [DataType(DataType.Currency)]
        public decimal Amount { get; set; }

        public Product Product { get; set; }

        public Order Order { get; set; }
    }
}
