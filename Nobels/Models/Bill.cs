using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Nobels.Models
{
    public partial class Bill
    {
        [Display(Name = "المعرف")]
        public int BillId { get; set; }
        [Display(Name = "رقم الفاتورة")]
        public string? BillNum { get; set; }
        [Display(Name = "تاريخ الفاتورة")]
        public DateTime? BillDate { get; set; }
        [Display(Name = "لغة الفاتورة")]
        public string? BillLanguage { get; set; }
        [Display(Name = "رقم العرض")]
        public int? QuotationId { get; set; }
        [Display(Name = "ملاحظات")]
        public string? Notes { get; set; }
        [Display(Name = "نص")]
        public string? BillText { get; set; }
        [Display(Name = "القيمة الكلية")]
        public double? TotalValue { get; set; }

        [Display(Name = "قيمة الخصم")]

        public string? discountType { get; set; }
       

        public double? Discount { get; set; }
        [Display(Name = "القيمة النهائية")]
        public string? FinalValue { get; set; }

        public virtual Quotation? Quotation { get; set; }
    }
}
