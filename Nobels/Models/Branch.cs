using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Nobels.Models
{
    public partial class Branch
    {
        public Branch()
        {
            Employees = new HashSet<Employee>();
        }
        [Display(Name = "المعرف")]
        public int BranchId { get; set; }
        [Display(Name = "الاسم")]
        //[Required(ErrorMessage = "هذا الحقل اجباري")]
        public string? BranchName { get; set; }
        [Display(Name = "الموقع")]
        //[Required(ErrorMessage = "هذا الحقل اجباري")]
        public string ? Location { get; set; }
        [Display(Name = "رقم الهاتف")]
        //[Required(ErrorMessage = "هذا الحقل اجباري")]
        public string? Phone { get; set; }
        [Display(Name = "الايميل")]
        //[Required(ErrorMessage = "هذا الحقل اجباري")]
        public string? Email { get; set; }
        [Display(Name = "العنوان")]
        //[Required(ErrorMessage = "هذا الحقل اجباري")]
        public string? Address { get; set; }
        [Display(Name = "المدينة")]
        //[Required(ErrorMessage = "هذا الحقل اجباري")]
        public int? CityId { get; set; }
        [Display(Name = "ملاحظات")]
        //[Required(ErrorMessage = "هذا الحقل اجباري")]
        public string? Notes { get; set; }
        [Display(Name = "صورة")]
        //[Required(ErrorMessage = "هذا الحقل اجباري")]
        public string? branchImg { get; set; }
        [Display(Name = "النوع")]
        //[Required(ErrorMessage = "هذا الحقل اجباري")]
        public int? BranchType { get; set; }

        public virtual City? City { get; set; }
       
        public virtual ICollection<Employee> Employees { get; set; }
        public virtual ICollection<Quotation> ?Quotations { get; set; }
    }
}
