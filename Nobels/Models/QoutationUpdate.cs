using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Nobels.Models
{
    public partial class QoutationUpdate
    {
        [Display(Name = "المعرف")]
        public int UpdateId { get; set; }
        [Display(Name = " الموظف")]
        public int? EmployeeId { get; set; }
        [Display(Name = "تاريخ التعديل")]
        public DateTime? ChangeDate { get; set; }
        [Display(Name = "التعديلات")]
        public string? Updates { get; set; }
        [Display(Name = "رقم العرض")]
        public int? QoutationId { get; set; }

        public virtual Employee? Employee { get; set; }
        public virtual Quotation? Qoutation { get; set; }
    }
}
