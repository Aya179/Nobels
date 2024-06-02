using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Nobels.Models
{
    public partial class Email
    {
        [Display(Name = "المعرف")]
        public int EmailId { get; set; }
        [Display(Name = "المستقبلين")]
        public string? Receivers { get; set; }
        [Display(Name = "تاريخ الإرسال")]
        public DateTime? Senddate { get; set; }
        [Display(Name = "الملاحظات")]
        public string? Notes { get; set; }
        [Display(Name = "نص الايميل")]
        public string? EmailText { get; set; }
    }
}
