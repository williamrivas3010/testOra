using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{


    /// <summary>
    /// 
    //[SSNValidator]
    //public string SSN { get; set; }
    /// </summary>
    public class SSNValidator : ValidationAttribute
    {

        public SSNValidator()
        {
            ErrorMessage = "Please enter an valid  SSN # in the format (###-##-####, ##-#######,#########)";
        }

        public override bool IsValid(object value)
        {
            if (!(value is String))
                return false;
            var val = value.ToString();

            if (string.IsNullOrWhiteSpace(val))
                return false;


            if (!val.Contains('-') && val.Where(c => char.IsDigit(c)).Count() == 9) return true;

            var dat = val.Split('-').ToList();

            
            return dat.Count() == 3 ?
                          (dat[0].Where(c => char.IsDigit(c)).Count() == 3 &&
                          dat[1].Where(c => char.IsDigit(c)).Count() == 2 &&
                          dat[2].Where(c => char.IsDigit(c)).Count() == 4) :
                          dat.Count() == 2 ?
                          (dat[0].Where(c => char.IsDigit(c)).Count() == 2 &&
                          dat[1].Where(c => char.IsDigit(c)).Count() == 7) : false;
            

        }
    }
}
