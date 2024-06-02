using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace Nobels.Models
{
    public partial class EmployeeRole
    {
        public EmployeeRole()
        {
            Employees = new HashSet<Employee>();
        }
        [Display(Name = "المعرف ")]
        public int RoleId { get; set; }
        [Display(Name = "السماحية ")]
        public string? RoleName { get; set; }
        [Display(Name = "ملاحظات ")]
        public string? Notes { get; set; }

        public virtual ICollection<Employee> Employees { get; set; }
    }
}
