using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models
{
    public class Customer :BaseModel
    {
        
        [Display(Name ="First Name")]
        [Required]
        public string FirstName { get; set; }

        [Display(Name ="Last Name")]
        [Required]
        public string LastName { get; set; }


        [DataType(DataType.PhoneNumber)]
        [Display(Name ="Phone")]
        public string Phone { get; set; }

        [Display(Name ="Company Name")]
        public string CompanyName { get; set; }


        [Index(IsUnique =true)]
        [MaxLength(200)]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }


        // aspnet Identity Implementation  Code First Configurations 
        [ForeignKey("ApplicationUser")]
        public string IdentityId { get; set; }

        public ApplicationUser ApplicationUser { get; set; }


        public List<Order> Orders { get; set; }
    }
}
