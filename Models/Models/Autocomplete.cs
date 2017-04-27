using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;


namespace Models
{
    public class Autocomplete
    {
        public int Id { get; set;}

        public string Name { get; set; }
    }

    public class AutocompleteStringKey
    {
        public string Id { get; set; }

        public string Name { get; set; }
    }

    public class ModalViewModel
    {
      

        public string ModalName { get; set; }

       
        public string CustomtargetId { get; set; }

        public bool LargeModal { get; set; }
    }

}