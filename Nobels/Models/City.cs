using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace Nobels.Models
{
    public partial class City
    {
        public City()
        {
            Branches = new HashSet<Branch>();
            Customers=new HashSet<Customer>();
        }


        public int CityId { get; set; }
        [Display(Name = "المدينة")]
        [Required(ErrorMessage = "هذا الحقل اجباري")]
        public string? CityName { get; set; }

        public virtual ICollection<Branch> Branches { get; set; }
        public virtual ICollection<Customer> Customers { get; set; }

    }
}
