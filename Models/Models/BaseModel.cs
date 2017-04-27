using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class BaseModel
    {

        public BaseModel()
        {
           // LastUpdate = DateTime.Now;

        }

        [Key]
        public int Id { get; set; }
        //public DateTime LastUpdate { get; set; }
        //public Guid ModifiedBy { get; set; }
    }
}
